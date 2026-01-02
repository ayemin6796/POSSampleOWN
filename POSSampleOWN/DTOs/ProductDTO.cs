using System.ComponentModel.DataAnnotations;

namespace POSSampleOWN.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateProductDTO
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }

    public class UpdateProductDTO
    {
        
        [MaxLength(150)]
        public string? Name { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ProductResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
        public ProductDTO? Data { get; set; }

    }
}
