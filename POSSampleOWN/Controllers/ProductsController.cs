using Microsoft.AspNetCore.Http.HttpResults;
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

        private IQueryable<Product> ActiveProductQuery => _dbContext.Products
            .AsNoTracking()
            .Where(product => product.IsActive == true);

        // GET: api/products/allProducts
        [HttpGet("allProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var lst = await _dbContext.Products.AsNoTracking()
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

        // GET: api/products/availableProductById/{id}
        [HttpGet("availableProductsById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await ActiveProductQuery.FirstOrDefaultAsync(p => p.Id == id);

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

        // GET: api/products/getAvailableProducts
        [HttpGet("availableProducts")]
        public async Task<IActionResult> GetAvailableProducts()
        {
            var availableProducts = await ActiveProductQuery
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


        // POST: api/products/createProduct
        [HttpPost("productCreate")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO createRequest)
        {
            if (string.IsNullOrEmpty(createRequest.Name) || 
                createRequest.Price <= 0 || 
                createRequest.StockQuantity <= 0)
            {
                return BadRequest(new ProductResponseDTO
                {
                    IsSuccess = false,
                    Message = "Name is required. Price and stock quantity must be positive."
                });
            }

            var newProduct = new Product 
            { 
                Name = createRequest.Name,
                Description = createRequest.Description,
                Price = createRequest.Price,
                StockQuantity = createRequest.StockQuantity,  
                CategoryId = createRequest.CategoryId,
                IsActive = true, 
                CreatedAt = DateTime.UtcNow
            };

            try
            {
              
                _dbContext.Products.Add(newProduct);
                var result = await _dbContext.SaveChangesAsync();

                var createdDto = new CreateProductDTO
                {
                    Name = createRequest.Name,
                    Description = createRequest.Description,
                    Price = createRequest.Price,
                    StockQuantity = createRequest.StockQuantity,
                    CategoryId = createRequest.CategoryId
                };

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
        [HttpPatch("productUpdate/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO updateRequest)
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

            if (updateRequest.Name is not null)
                product.Name = updateRequest.Name;

            if (updateRequest.Description is not null)
                product.Description = updateRequest.Description;

            if (updateRequest.Price > 0)
                product.Price = updateRequest.Price;

            if (updateRequest.StockQuantity >= 0)
                product.StockQuantity = updateRequest.StockQuantity;

            if (updateRequest.CategoryId != 0)
                product.CategoryId = updateRequest.CategoryId;

            if (updateRequest.IsActive.HasValue)
                product.IsActive = updateRequest.IsActive.Value;

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
        [HttpDelete("productSoftDelete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == id);

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

        
        // PATCH: api/products/setProductStatus/{id}
        //[HttpPatch("ProductStatus/{id}")]
        //public async Task<IActionResult> SetProductStatus(int id, [FromBody] ProductDTO request)
        //{
        //    var product = await _dbContext.Products
        //        .FirstOrDefaultAsync(p => p.Id == id && !p.IsActive);

        //    if (product is null)
        //    {
        //        return NotFound(new ProductResponseDTO
        //        {
        //            IsSuccess = false,
        //            Message = "Product not found."
        //        });
        //    }

        //    product.IsActive = request.IsActive;
        //    product.UpdatedAt = DateTime.UtcNow;

        //    var result = await _dbContext.SaveChangesAsync() > 0;

        //    return Ok(new ProductResponseDTO        
        //    {
        //        IsSuccess = result,
        //        Message = result ? "Product status updated successfully!" : "Failed to update product status."
        //    });
        //}



    }
}
