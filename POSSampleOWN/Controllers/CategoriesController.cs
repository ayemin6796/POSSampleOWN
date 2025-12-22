using Microsoft.AspNetCore.Mvc;
using POSSampleOWN.Data;
using POSSampleOWN.DTOs;
using POSSampleOWN.Models;

namespace POSSampleOWN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly POSDbContext dbContext; // Database context

        // Constructor
        public CategoriesController(POSDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/Categories
        // Executing all categories with lists using DTOs
        [HttpGet]
        public IActionResult GetAll()
        {
            // Get all categories
            var categories = dbContext.Categories.ToList();

            // Map Domain Models to DTOs
            var categoryDto = new List<CategoryDTO>();
            foreach (var category in categories)
            {
                categoryDto.Add(new CategoryDTO()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                });
            }
            return Ok(categoryDto);
        }

        // GET: api/Categories/id
        // Executing category by ID using DTO
        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            // Get category by ID
            var category = dbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            var categoryDto = new CategoryDTO()
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return Ok(categoryDto);
        }

        // POST: api/Categories
        // Creating a new category using DTO
        [HttpPost]
        public IActionResult Create([FromBody] CreateCategoryDTO createCategoryDTO)
        {
            // Map DTO to Domain Model
            var category = new Category
            {
                Name = createCategoryDTO.Name,
                Description = createCategoryDTO.Description
            };

            // Save to database
            dbContext.Categories.Add(category);
            dbContext.SaveChanges();

            // Map Domain model back to DTO
            var categoryDto = new CategoryDTO()
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, categoryDto);
        }
    }
}
