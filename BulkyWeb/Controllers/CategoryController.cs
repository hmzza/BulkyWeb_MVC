using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();

            return View(objCategoryList);
        }

        //get action
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //custom validations
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Dislpay order and Name can't be the same");
            }
            
            if (obj.Name!=null && obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Test is invalid value");
            }

            //if obj is valid it will go to category.cs, it will check whatever is required and it should be populated accordingly
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj); //write category object to the category table
                _db.SaveChanges();
                //to show successful dialogue on screen
                TempData["success"] = "Category created successfully!";
                return RedirectToAction("Index");
            }

            return View();            
        }

        //get action
        public IActionResult Edit(int? id)
        {
            if (id == null || id==0) { 
                return NotFound();
            }

            Category? categoryFromDb = _db.Categories.Find(id);
            //Category? categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {

            //if obj is valid it will go to category.cs, it will check whatever is required and it should be populated accordingly
            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj); //write category object to the category table
                _db.SaveChanges();
                TempData["success"] = "Category updated successfully!";
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

            Category? categoryFromDb = _db.Categories.Find(id);
            //Category? categoryFromDb = _db.Categories.FirstOrDefault(c => c.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {

            Category? obj = _db.Categories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);

            //_db.Categories.Update(obj); //write category object to the category table
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully!";          
            return RedirectToAction("Index");
        }
    }
}
