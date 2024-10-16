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
        private readonly IWebHostEnvironment _webHostEnvironment; //FOR uploading images
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductsList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(objProductsList);
        }

        //get action
        //upsert is combination of create and insert
        public IActionResult Upsert(int? id)
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

            if(id==null || id==0)
            {
                //that means it is create functionality
                return View(productVM);

            }
            else
            {
                //that means it is update functionality
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }


        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            //if obj is valid it will go to Product.cs, it will check whatever is required and it should be populated accordingly
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath; //this will give us path of root folder which is wwwroot
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); //that will give a random name to file
                    string prodcutPath = Path.Combine(wwwRootPath, @"images\product"); //that will give path inside product folder of images

                    // for updating image 
                    if (!string.IsNullOrEmpty(productVM.Product.ImageURL)) { 
                        //delete the old image
                        //trimmig \ from path
                        var oldImagePath = Path.Combine(wwwRootPath,productVM.Product.ImageURL.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath)) { 
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    //saving image
                    using (var fileStream = new FileStream(Path.Combine(prodcutPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageURL = @"\images\product\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product); //write Product object to the Product table
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                
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

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = objProductsList });
        }

        #endregion

    }
}
