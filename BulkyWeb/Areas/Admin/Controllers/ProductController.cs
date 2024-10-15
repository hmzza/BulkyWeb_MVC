using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{

    [Area("Admin")]
    //THIS CONTROLLER IS IN ADMIN AREA
    public class ProductController : Controller
    {
        //private readonly ApplicationDbContext _db;
        //now we dont need ApplicationDbContext because we have Repositories

        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objProductsList = _unitOfWork.Product.GetAll().ToList();

            return View(objProductsList);
        }

        //get action
        public IActionResult Create()
        {

            // CONVERTING CATEGORY INTO IENUMERABLE,, THIS IS USING PROJECTION
            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.Id.ToString()
            //});


            // USING VIEWBAG TO SEND CATEGORY DATA TO PRODUCTS
            //ViewBag.CategoryList = CategoryList; 

            // USING VIEWDATA TO SEND CATEGORY DATA TO PRODUCTS
            //ViewData["CategoryList"] = CategoryList;

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            }; 

            return View(productVM);
        }
        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            //if obj is valid it will go to Product.cs, it will check whatever is required and it should be populated accordingly
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(productVM.Product); //write Product object to the Product table
                _unitOfWork.Save();
                //to show successful dialogue on screen
                TempData["success"] = "Product created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }

            
        }

        //get action
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            //Product? ProductFromDb = _db.Categories.Find(id);
            //Product? ProductFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);

            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Product obj)
        {

            //if obj is valid it will go to Product.cs, it will check whatever is required and it should be populated accordingly
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(obj);
                _unitOfWork.Save();
                //_db.Categories.Update(obj); //write Product object to the Product table
                //_db.SaveChanges();
                TempData["success"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }

            return View();
        }

        //get action
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }


            Product? ProductFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            //Product? ProductFromDb = _db.Categories.Find(id);
            //Product? ProductFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);

            if (ProductFromDb == null)
            {
                return NotFound();
            }
            return View(ProductFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {

            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
            //Product? obj = _db.Categories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            //_db.Categories.Remove(obj);

            //_db.Categories.Update(obj); //write Product object to the Product table
            //_db.SaveChanges();
            TempData["success"] = "Product deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
