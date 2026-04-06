
namespace Nilearn.Application.Features.Category.DTOs
{
    public record CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? IconClass { get; set; }
        public bool IsActive { get; set; }
    }
}
