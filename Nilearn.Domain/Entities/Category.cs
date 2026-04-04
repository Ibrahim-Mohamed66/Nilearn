namespace Nilearn.Domain.Entities;

public class Category : BaseEntity
{

    public string Name { get; set; }

    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public bool IsActive { get; set; } = true;
}