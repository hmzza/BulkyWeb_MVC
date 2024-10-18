using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    //THIS CONTROLLER IS IN ADMIN AREA

    //doing this so that no one can go by pasting url in browser
    //[Authorize(Roles = StaticDetails.Role_Admin)]
    public class CompanyController : Controller
    {
        //private readonly ApplicationDbContext _db;
        //now we dont need ApplicationDbContext because we have Repositories

        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objCompanysList = _unitOfWork.Company.GetAll().ToList();

            return View(objCompanysList);
        }

        //get action
        //upsert is combination of create and insert
        public IActionResult Upsert(int? id)
        {


            if(id==null || id==0)
            {
                //that means it is create functionality
                return View(new Company());

            }
            else
            {
                //that means it is update functionality
                Company companyObj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);
            }


        }
        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            //if obj is valid it will go to Company.cs, it will check whatever is required and it should be populated accordingly
            if (ModelState.IsValid)
            {
                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(CompanyObj); //write Company object to the Company table
                }
                else
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }
                
                _unitOfWork.Save();
                //to show successful dialogue on screen
                TempData["success"] = "Company created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(CompanyObj);
            }

            
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanysList = _unitOfWork.Company.GetAll().ToList();
            return Json(new {data = objCompanysList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyToBeDeleted = _unitOfWork.Company.Get(u=>u.Id == id);   
            if (companyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
          
            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted Successful!" });
        }

        #endregion


    }
}
