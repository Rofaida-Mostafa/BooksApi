using BooksApi.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace BooksApi.Areas.Customer.Controllers
{
    [Area(SD.CustomerArea)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IRepository<Category> _categoryRepository;

        public ValuesController(IBookRepository bookRepository, IRepository<Category> categoryRepository)
        {
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery]BookFilterRequest bookFilterRequest, int page = 1)
        {
            var books = (await _bookRepository.GetAllAsync(includes: [e => e.Category])).AsQueryable();

            // Filter
            if (bookFilterRequest.BookName is not null)
            {
                books = books.Where(e => e.AuthorName.Contains(bookFilterRequest.BookName));
            }

            if (bookFilterRequest.CategoryId is not null)
            {
                books = books.Where(e => e.CategoryId == bookFilterRequest.CategoryId);
            }
          
            // Pagination
            double totalPages = Math.Ceiling(books.Count() / 8.0); // 3.1 => 4
            int currentPage = page;

            books = books.Skip((page - 1) * 8).Take(8);

            // Returned Data
            var categories = await _categoryRepository.GetAllAsync();

            return Ok(new
            {
                books,
                bookFilterRequest.BookName,
                bookFilterRequest.CategoryId,
                bookFilterRequest.Price,
                totalPages,
                currentPage,
                categories
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            var book = await _bookRepository.GetOne(e => e.Id == id, includes: [e => e.Category]);

            if (book is null)
                return NotFound();

            await _bookRepository.comitChanges();

            // Related books
            var relatedbooks = (await _bookRepository.GetAllAsync(e => e.CategoryId == book.CategoryId && e.Id != book.Id, includes: [e => e.Category])).Skip(0).Take(4);

            // Similar books
            var similarbooks = (await _bookRepository.GetAllAsync(e => e.AuthorName.Contains(book.AuthorName) && e.Id != book.Id, includes: [e => e.Category])).Skip(0).Take(4);

            // Return Data
            BookWithRelatedResponse bookWithRelatedResponse = new()
            {
                book = book,
                RelatedBooks = relatedbooks.ToList(),
                SimilarBooks = similarbooks.ToList()
            };

            return Ok(bookWithRelatedResponse);
        }
    }
}
