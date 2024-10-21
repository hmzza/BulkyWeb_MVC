using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db) //by saying base(db) we are passing all of the information to the base class Repository<Category>
        {
            _db = db;
        }
        

        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }
    }
}
