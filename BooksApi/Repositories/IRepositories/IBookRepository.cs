using System.Linq.Expressions;

namespace BooksApi.Repositories.IRepositories
{
    public interface IBookRepository : IRepository<Book>
    {
            Task AddRangeAsync(List<Book> books);
        
    }
}
