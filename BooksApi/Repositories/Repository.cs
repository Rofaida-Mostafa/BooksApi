using  BooksApi.Repositories.IRepositories;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;


namespace BooksApi.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {


        private ApplicationDbContext _context; //= new(); // database
            private DbSet<T> _db; // for models
        public Repository(ApplicationDbContext context)
            {
            _context = context;
               _db = _context.Set<T>();

            }
            public async Task<T> CreateAsync(T entity)
            {
                await _db.AddAsync(entity);
            return entity;

            }

            public void Update(T entity)
            {
                _db.Update(entity);
            }

            public void Delete(T entity)
            {
                _db.Remove(entity);
            }

            public async Task comitChanges()
            {

                await _context.SaveChangesAsync();

            }
            public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null,
                Expression<Func<T, object>>?[]  includes = null, bool tracked= true)
            {
                var entities = _db.AsQueryable();
                if (expression != null) entities = entities.Where(expression);

            if (includes is not null) 
            {
                foreach(var item in includes)
                {
                    entities = entities.Include(item);
                }
            }

            if (!tracked) 
            {
                
                    entities = entities.AsNoTracking();
                
            }

                return await entities.ToListAsync();
            }

            public async Task<T?> GetOne(Expression<Func<T, bool>> expression, Expression<Func<T, object>>?[] includes = null)
            {

              return (await GetAllAsync(expression, includes)).FirstOrDefault();

            }

        public Task DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
            return Task.CompletedTask;
        }
    }
}



