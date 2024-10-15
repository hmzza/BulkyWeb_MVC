using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db) //by saying base(db) we are passing all of the information to the base class Repository<Category>
        {
            _db = db;
        }


        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u=>u.Id == obj.Id);
            if (objFromDb != null) 
            { 
                objFromDb.Title = objFromDb.Title;
                objFromDb.ISBN = objFromDb.ISBN;
                objFromDb.Price = objFromDb.Price;
                objFromDb.ListPrice = objFromDb.ListPrice; 
                objFromDb.Price50 = objFromDb.Price50;
                objFromDb.Price100 = objFromDb.Price100;
                objFromDb.Description = objFromDb.Description;
                objFromDb.CategoryID = objFromDb.CategoryID;
                objFromDb.Author = objFromDb.Author;
                if (obj.ImageURL != null) {
                    objFromDb.ImageURL = obj.ImageURL;
                }
            }
        }
    }
}
