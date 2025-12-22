using Microsoft.AspNetCore.Mvc;
using POSSampleOWN.Data;
using POSSampleOWN.DTOs;
using POSSampleOWN.Models;

namespace POSSampleOWN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly POSDbContext dbContext; // Database context

        // Constructor
        public ProductsController(POSDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/Products
        // Executing all products with lists using DTOs
        [HttpGet]
        public IActionResult GetAll()
        {
            // Get all products
            var products = dbContext.Products.ToList();

            // Map Domain Models to DTOs
            var productDto = new List<ProductDTO>();
            foreach (var product in products)
            {
                productDto.Add(new ProductDTO()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    CategoryId = product.CategoryId,
                    IsActive = product.IsActive
                });
            }
            return Ok(productDto);
        }

        // GET: api/Products/id
        // Executing product by ID using DTO
        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            // Get product by ID
            var product = dbContext.Products.FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive
            };

            return Ok(productDto);
        }

        // POST: api/Products
        // Creating a new product using DTO
        [HttpPost]
        public IActionResult Create([FromBody] CreateProductDTO createProductDTO)
        {
            // Map DTO to Domain Model
            var product = new Product
            {
                Name = createProductDTO.Name,
                Description = createProductDTO.Description,
                Price = createProductDTO.Price,
                StockQuantity = createProductDTO.StockQuantity,
                CategoryId = createProductDTO.CategoryId,
            };

            // Save to database
            dbContext.Products.Add(product);
            dbContext.SaveChanges();

            // Map Domain model back to DTO
            var productDto = new ProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive
            };

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, productDto);
        }
    }
}
