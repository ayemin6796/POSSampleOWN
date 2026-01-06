using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSSampleOWN.Data;
using POSSampleOWN.DTOs;
using POSSampleOWN.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace POSSampleOWN.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly POSDbContext _dbContext;

        public CategoriesController(POSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/categories/getAllCategories
        [HttpGet("getAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _dbContext.Categories
                .AsNoTracking()
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/categories/getCategoryById/{id}
        [HttpGet("getCategoryById/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            if ()

            var category = await _dbContext.Categories
                .AsNoTracking()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
            {
                return NotFound(new CategoryResponseDTO
                {
                    IsSuccess = false,
                    Message = "Category not found."
                });
            }

            var dto = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return Ok(new CategoryResponseDTO
            {
                IsSuccess = true,
                Message = "Category retrieved successfully.",
                Data = dto
            });
        }

        // POST: api/categories/createCategory
        [HttpPost("createCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(new CategoryResponseDTO
                {
                    IsSuccess = false,
                    Message = "Name is required."
                });
            }

            //check duplicate category name
            //not very sure that's why i commented out
            //if (_dbContext.Categories.Any(c => c.Name == request.Name.Trim()))
            //{
            //    return BadRequest(new CategoryResponseDto
            //    {
            //        IsSuccess = false,
            //        Message = "A category with the same name already exists."
            //    });
            //}

            var newCategory = new Category
            {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
          
                await _dbContext.Categories.AddAsync(newCategory);
                var result = await _dbContext.SaveChangesAsync();

                var data = new CategoryDTO
                {
                    Id = newCategory.Id,
                    Name = newCategory.Name,
                    Description = newCategory.Description
                };

                return Ok(new CategoryResponseDTO
                {
                    IsSuccess = result > 0,
                    Message = result > 0 ? "Category created successfully." : "Failed to create category.",
                    Data = data
                });
            }
            catch
            {
                return StatusCode(500, new CategoryResponseDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while creating the category."
                });
            }
        }

        // PATCH: api/categories/updateCategory/{id}
        [HttpPatch("updateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDTO request)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
            {
                return NotFound(new CategoryResponseDTO
                {
                    IsSuccess = false,
                    Message = "Category not found."
                });
            }

            if (!string.IsNullOrEmpty(request.Name))
                category.Name = request.Name;

            if (request.Description != null)
                category.Description = request.Description;

            category.UpdatedAt = DateTime.UtcNow;

            try
            {
                var updateResult = await _dbContext.SaveChangesAsync() > 0;
                return Ok(new CategoryResponseDTO
                {
                    IsSuccess = updateResult,
                    Message = updateResult ? "Category updated successfully." : "No changes were made."
                });
            }
            catch
            {
                return StatusCode(500, new CategoryResponseDTO
                {
                    IsSuccess = false,
                    Message = "Database error occurred while updating the category."
                });
            }
        }

        // DELETE: api/categories/deleteCategory/{id}
        //[HttpDelete("deleteCategory/{id}")]
        //public async Task<IActionResult> DeleteCategory(int id)
        //{
        //    var category = await _dbContext.Categories
        //        .Include(c => c.Products)
        //        .FirstOrDefaultAsync(c => c.Id == id);

        //    if (category is null)
        //    {
        //        return NotFound(new CategoryResponseDTO
        //        {
        //            IsSuccess = false,
        //            Message = "Category not found."
        //        });
        //    }

        //    if (category.Products != null && category.Products.Any())
        //    {
        //        return BadRequest(new CategoryResponseDTO
        //        {
        //            IsSuccess = false,
        //            Message = "Category cannot be deleted because it contains products. Remove or products first."
        //        });
        //    }

        //    _dbContext.Categories.Remove(category);


        //    try
        //    {
        //        var removed = await _dbContext.SaveChangesAsync() > 0;
        //        return Ok(new CategoryResponseDTO
        //        {
        //            IsSuccess = removed,
        //            Message = removed ? "Category deleted successfully." : "Failed to delete category."
        //        });
        //    }
        //    catch
        //    {
        //        return StatusCode(500, new CategoryResponseDTO
        //        {
        //            IsSuccess = false,
        //            Message = "An error occurred while deleting the category."
        //        });
        //    }
        //}
    }
}