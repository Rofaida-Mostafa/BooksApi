using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksApi.Areas.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [Area(SD.AdminArea)]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private IRepository<Category> _category_Repository;
        public CategoriesController(IRepository<Category> category_Repository)
        {
            _category_Repository = category_Repository;
        }
        #region Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _category_Repository.GetAllAsync();
            return Ok(categories);
        }
        #endregion

        #region Create
        [HttpPost]

        public async Task<IActionResult> Create(CategoriesRequest categoriesRequest)
        {
                await _category_Repository.CreateAsync(categoriesRequest.Adapt<Category>());
                await _category_Repository.comitChanges();
             
            return Ok(new
            {
                success = true,
                message = "Category created successfully"
            });
        }
        #endregion     

        [HttpGet("{id}")]
        #region Get - Edit
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _category_Repository.GetOne(e => e.Id == id);
            if (category == null) return NotFound();
            return Ok(category);
        }
        #endregion

        #region Update
        [HttpPut("{id}")]

        public async Task<IActionResult> Update(int id, CategoriesRequest categoriesRequest)
        {


          var categoryInDb= await _category_Repository.GetOne(e => e.Id == id);

            if (categoryInDb == null) return NotFound();

            categoryInDb.Name= categoriesRequest.Name ;
            categoryInDb.Description= categoriesRequest.Description ;
            categoryInDb.Status= categoriesRequest.Status ;
            await _category_Repository.comitChanges();

            return Ok(new
            {
                success = true,
                message = "Update Category successfully"
            });
        }
        #endregion

        #region Delete
        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {
          var categoryInDb= await _category_Repository.GetOne(e => e.Id == id);

            if (categoryInDb == null) return NotFound();

            _category_Repository.Delete(categoryInDb);

            await _category_Repository.comitChanges();

            return Ok(new
            {
                success = true,
                message = "Delete Category successfully"
            });
        }
        #endregion
    }
}
