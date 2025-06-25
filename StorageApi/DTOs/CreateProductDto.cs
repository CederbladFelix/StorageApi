using System.ComponentModel.DataAnnotations;

namespace StorageApi.DTOs
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = null!;
        [Range(1, int.MaxValue)]
        public int Price { get; set; }
        [Required]
        public string Category { get; set; } = null!;
        [StringLength(3, MinimumLength = 1)]
        [Required]
        public string Shelf { get; set; } = null!;
        [Range(1, 100)]
        public int Count { get; set; }
        public string? Description { get; set; }
    }
}
