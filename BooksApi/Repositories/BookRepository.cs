
using System.Linq.Expressions;
namespace BooksApi.Repositories
{
   
        public class BookRepository : Repository<Book>, IBookRepository
        {
        private ApplicationDbContext _context;

            public BookRepository(ApplicationDbContext context) : base(context)
            {
                _context = context;
            }

            public async Task AddRangeAsync(List<Book> products)
            {
                await _context.Books.AddRangeAsync(products);
            }

        
    }
}
