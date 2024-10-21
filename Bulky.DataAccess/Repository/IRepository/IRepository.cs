using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    //we uses generic class T because right now we have categories but in futuure we will be needing products etc so for that we are doing it generic here
    public interface IRepository<T> where T : class
    {
        // T - will be category or any other generic model on which we perform CRUD operation or rather we want to interact DBContext
        
        
        // lets say (T is Category)
        // what will be all operation?
        // we need to retrieve all categorys to diplay them

        IEnumerable<T> GetAll(string? includeProperties = null);    // for all 
        T Get(Expression<Func<T,bool>> filter, string? includeProperties = null, bool tracked = false);  // fetching single record
        void Add(T entity);     // for adding record
        void Remove(T entity);  // for deleting record
        void RemoveRange(IEnumerable<T> entities);  //deleteing in some range

        //WE WILL NOT HAVE UPDATE IN INTERFACE BECAUSE LOGIC CAN BE DIFFERENT FOR PRODUCTS AND CATEGORIES ETC
        //void Update(T entity);  // for updating record





    }
}
