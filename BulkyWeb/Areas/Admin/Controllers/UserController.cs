using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    //THIS CONTROLLER IS IN ADMIN AREA

    //doing this so that no one can go by pasting url in browser
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class UserController : Controller
    {
        //private readonly ApplicationDbContext _db;
        //now we dont need ApplicationDbContext because we have Repositories

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }       

        public IActionResult RoleManagement(string userId)
        {
            string RoleId = _db.UserRoles.FirstOrDefault(u=>u.UserId == userId).RoleId;

            RoleManagementVM RoleVM = new RoleManagementVM()
            {
                ApplicationUser = _db.ApplicationUser.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
                RoleList = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _db.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u=>u.Id == RoleId).Name;

            return View(RoleVM);
        }


        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVM.ApplicationUser.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;

            if(!(roleManagementVM.ApplicationUser.Role == oldRole))
            {
                // a role was updated
                ApplicationUser applicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == roleManagementVM.ApplicationUser.Id);

                if (roleManagementVM.ApplicationUser.Role == StaticDetails.Role_Company) {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId; 
                }

                if(oldRole == StaticDetails.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _db.SaveChanges();
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();    //delete old role
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();   //add new role
            }

 

            return RedirectToAction("Index");
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUser.Include(u=>u.Company).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(u=>u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u=>u.Id == roleId).Name;

                if(user.Company == null)
                {
                    user.Company = new Company() { Name=""};
                }
            }

            return Json(new {data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful!" });
        }

        #endregion


    }
}
