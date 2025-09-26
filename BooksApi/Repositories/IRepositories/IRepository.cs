
using System.Linq.Expressions;

namespace  BooksApi.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {

        Task<T> CreateAsync(T entity);
      
        void Update(T entity);
       
        void Delete(T entity);
        
        Task comitChanges();
        
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null, bool tracked = true);
        
         Task<T?> GetOne(Expression<Func<T, bool>> expression, Expression<Func<T, object>>?[] includes = null);
        Task DeleteRange(IEnumerable<T> entities);
    }
}
