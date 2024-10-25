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
    [Authorize(Roles = StaticDetails.Role_Admin)]
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
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties:"ProductImages");
                return View(productVM);
            }


        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            //if obj is valid it will go to Product.cs, it will check whatever is required and it should be populated accordingly
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product); //write Product object to the Product table
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath; //this will give us path of root folder which is wwwroot
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); //that will give a random name to file
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath); //that will give path inside product folder of images

                        if (!Directory.Exists(finalPath)) { 
                            Directory.CreateDirectory(finalPath);
                        }

                        ////saving image
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if(productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }

                        productVM.Product.ProductImages.Add(productImage); 
                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
                }


                //to show successful dialogue on screen
                TempData["success"] = "Product created/updated successfully!";
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


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = objProductsList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var prodToBeDeleted = _unitOfWork.Product.Get(u=>u.Id == id);   
            if (prodToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            //var oldImagePath =
            //    Path.Combine(_webHostEnvironment.WebRootPath, prodToBeDeleted.ImageURL.TrimStart('\\'));

            //if (System.IO.File.Exists(oldImagePath))
            //{
            //    System.IO.File.Delete(oldImagePath);
            //}

            _unitOfWork.Product.Remove(prodToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted Successful!" });
        }

        #endregion


    }
}
