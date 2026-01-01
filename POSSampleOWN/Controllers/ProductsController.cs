using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSSampleOWN.Data;
using POSSampleOWN.DTOs;
using POSSampleOWN.Models;

namespace POSSampleOWN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly POSDbContext _dbContext; // Database context

        // Constructor
        public ProductsController(POSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Products
        // Executing all products with lists using DTOs
        //[HttpGet]
        //public async Task<IActionResult> GetAllProducts()
        //{
        //    // Get all products
        //    var products = dbContext.Products.ToList();

        //    // Map Domain Models to DTOs
        //    var productDto = new List<ProductDTO>();
        //    foreach (var product in products)
        //    {
        //        productDto.Add(new ProductDTO()
        //        {
        //            Id = product.Id,
        //            Name = product.Name,
        //            Description = product.Description,
        //            Price = product.Price,
        //            StockQuantity = product.StockQuantity,
        //            CategoryId = product.CategoryId,
        //            IsActive = product.IsActive
        //        });
        //    }
        //    return Ok(productDto);
        //}

        //// GET: api/Products/id
        //// Executing product by ID using DTO
        //[HttpGet]
        //[Route("{id:int}")]
        //public IActionResult GetById([FromRoute] int id)
        //{
        //    // Get product by ID
        //    var product = dbContext.Products.FirstOrDefault(c => c.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    var productDto = new ProductDTO()
        //    {
        //        Id = product.Id,
        //        Name = product.Name,
        //        Description = product.Description,
        //        Price = product.Price,
        //        StockQuantity = product.StockQuantity,
        //        CategoryId = product.CategoryId,
        //        IsActive = product.IsActive
        //    };

        //    return Ok(productDto);
        //}

        //// POST: api/Products
        //// Creating a new product using DTO
        //[HttpPost]
        //public IActionResult Create([FromBody] CreateProductDTO createProductDTO)
        //{
        //    // Map DTO to Domain Model
        //    var product = new Product
        //    {
        //        Name = createProductDTO.Name,
        //        Description = createProductDTO.Description,
        //        Price = createProductDTO.Price,
        //        StockQuantity = createProductDTO.StockQuantity,
        //        CategoryId = createProductDTO.CategoryId,
        //    };

        //    // Save to database
        //    _dbContext.Products.Add(product);
        //    _dbContext.SaveChanges();

        //    // Map Domain model back to DTO
        //    var productDto = new ProductDTO()
        //    {
        //        Id = product.Id,
        //        Name = product.Name,
        //        Description = product.Description,
        //        Price = product.Price,
        //        StockQuantity = product.StockQuantity,
        //        CategoryId = product.CategoryId,
        //        IsActive = product.IsActive
        //    };

        //    return CreatedAtAction(nameof(GetById), new { id = product.Id }, productDto);
        //}

        private IQueryable<Product> ProductQuery => _dbContext.Products
            .AsNoTracking()
            .Where(product => product.IsActive == true);

        // GET: api/products/getAllProducts
        [HttpGet("getAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var lst = await ProductQuery
                .Select(product => new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    CategoryId = product.CategoryId
                })
                .ToListAsync();

            return Ok(lst);
        }

        // GET: api/products/getProductById/{id}
        [HttpGet("getProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await ProductQuery.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return NotFound(new ProductResponseDTO
                {
                    IsSuccess = false,
                    Message = "Product not found."
                });
            }

            var result = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId
            };

            return Ok(new ProductResponseDTO
            {
                IsSuccess = true,
                Message = "Product retrieved successfully.",
                Data = result
            });
        }

        // POST: api/products/createProduct
        [HttpPost("createProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO request)
        {
            if (string.IsNullOrEmpty(request.Name) || request.Price <= 0 || request.Price.Equals(null))
            {
                return BadRequest(new ProductResponseDTO
                {
                    IsSuccess = false,
                    Message = "Name and Price are required."
                });
            }

            try
            {
                var newProduct = new Product
                {
                    Id = request.Id,
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    CategoryId = request.CategoryId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _dbContext.Products.AddAsync(newProduct);
                var result = await _dbContext.SaveChangesAsync();

                return Ok(new ProductResponseDTO
                {
                    IsSuccess = result > 0,
                    Message = result > 0 ? "Product created successfully." : "Failed to create product."
                });
            }
            catch
            {
                return StatusCode(500, new ProductResponseDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while creating the product."
                });
            }
        }

        // PATCH: api/products/updateProduct/{id}
        [HttpPatch("updateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO request)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return NotFound(new ProductResponseDTO
                {
                    IsSuccess = false,
                    Message = "Product not found."
                });
            }

            if (!string.IsNullOrEmpty(request.Name))
                product.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Description))
                product.Description = request.Description;

            if (request.Price > 0)
                product.Price = request.Price;

            if (request.StockQuantity >= 0)
                product.StockQuantity = request.StockQuantity;

            if (request.CategoryId != 0)
                product.CategoryId = request.CategoryId;

            product.IsActive = request.IsActive;

            product.UpdatedAt = DateTime.UtcNow;

            var result = await _dbContext.SaveChangesAsync() > 0;

            try 
            {
                return Ok(new ProductResponseDTO
                {
                    IsSuccess = result,
                    Message = result ? "Product updated successfully!" : "Failed to update product."
                });
            }
            catch             
            {
                return StatusCode(500, new ProductResponseDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while updating the product."
                });
            }

        }

        // DELETE: api/products/deleteProduct/{id}
        [HttpDelete("deleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsActive);

            if (product is null)
            {
                return NotFound(new ProductResponseDTO
                {
                    IsSuccess = false,
                    Message = "Product not found."
                });
            }

            // Soft delete
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            var result = await _dbContext.SaveChangesAsync() > 0;

            return Ok(new ProductResponseDTO
            {
                IsSuccess = result,
                Message = result ? "Product deleted successfully!" : "Failed to delete product."
            });
        }

        // GET: api/products/getAvailableProducts
        [HttpGet("getAvailableProducts")]
        public async Task<IActionResult> GetAvailableProducts()
        {
            var availableProducts = await ProductQuery
                .Where(p => p.IsActive && p.StockQuantity > 0)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.StockQuantity,
                    p.CategoryId
                })
                .ToListAsync();

            return Ok(availableProducts);
        }

        // PATCH: api/products/setProductStatus/{id}
        [HttpPatch("setProductStatus/{id}")]
        public async Task<IActionResult> SetProductStatus(int id, [FromBody] ProductDTO request)
        {
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsActive);

            if (product is null)
            {
                return NotFound(new ProductResponseDTO
                {
                    IsSuccess = false,
                    Message = "Product not found."
                });
            }

            product.IsActive = request.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            var result = await _dbContext.SaveChangesAsync() > 0;

            return Ok(new ProductResponseDTO        
            {
                IsSuccess = result,
                Message = result ? "Product status updated successfully!" : "Failed to update product status."
            });
        }
    }
}
