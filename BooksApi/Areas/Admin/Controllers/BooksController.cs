using BooksApi.DTOs.Response;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksApi.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private IBookRepository _bookRepository;
        private IRepository<Category> _category_Repository;

        public BooksController(IBookRepository bookRepository,
            IRepository<Category> category_Repository)
        {
            _bookRepository = bookRepository;
            _category_Repository = category_Repository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Book> books = await _bookRepository.GetAllAsync(includes: [m => m.Category]);
            var bookResponse = books.Adapt<List<BookResponse>>();
            return Ok(bookResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookRepository.GetOne(e => e.Id == id);

            if (book is null)
                return RedirectToAction(SD.NotFoundPage, SD.HomeController);



            return Ok(book.Adapt<BookResponse>());
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] BookCreateRequest bookCreateRequest)
        {
            if (bookCreateRequest.ImgUrlFile is not null && bookCreateRequest.ImgUrlFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(bookCreateRequest.ImgUrlFile.FileName);
                // 0924fdsfs-d429-fskdf-jsd230-423.png

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                // Save Img in wwwroot
                using (var stream = System.IO.File.Create(filePath))
                {
                    await bookCreateRequest.ImgUrlFile.CopyToAsync(stream);
                }

                // Save img in DB
                var Book = bookCreateRequest.Adapt<Book>();
                Book.ImgUrl = fileName;

                // Save in DB
                var BookReturned = await _bookRepository.CreateAsync(Book);
                await _bookRepository.comitChanges();

                return CreatedAtAction(nameof(Details), new { id = BookReturned.Id }, new
                {
                    msg = "Created Book Successfully"
                });
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] BookUpdateRequest BookUpdateRequest)
        {

            var BookInDB = await _bookRepository.GetOne(e => e.Id == id);

            if (BookInDB is null)
                return BadRequest();

            var book = BookUpdateRequest.Adapt<Book>();
            book.Id = id;

            if (BookUpdateRequest.ImgUrlFile is not null && BookUpdateRequest.ImgUrlFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(BookUpdateRequest.ImgUrlFile.FileName);
                // 0924fdsfs-d429-fskdf-jsd230-423.png

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                // Save Img in wwwroot
                using (var stream = System.IO.File.Create(filePath))
                {
                    await BookUpdateRequest.ImgUrlFile.CopyToAsync(stream);
                }

                // Delete old img from wwwroot
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", BookInDB.ImgUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Update img name in DB
                book.ImgUrl = fileName;
            }
            else
            {
                book.ImgUrl = BookInDB.ImgUrl;
            }

            // Update in DB
            _bookRepository.Update(book);
            await _bookRepository.comitChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _bookRepository.GetOne(e => e.Id == id);

            if (product is null)
                return NotFound();

            // Delete old img from wwwroot
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.ImgUrl);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            // Remove in DB
            _bookRepository.Delete(product);
            await _bookRepository.comitChanges();

            return NoContent();
        }
    }
}
