using System.ComponentModel.DataAnnotations;

namespace POSSampleOWN.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class CreateCategoryDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(250)]
        public string? Description { get; set; }
    }

    public class UpdateCategoryDTO
    {
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(250)]
        public string? Description { get; set; }
    }

    public class CategoryResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public CategoryDTO? Data { get; set; }
    }
}
