namespace Nilearn.Domain.Entities;
public class Instructor
{
    public int Id { get; set; }
    public Guid AppUserId { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string? Headline { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool IsApproved { get; set; } = false;
    public decimal EarningBalance { get; set; } = 0;
    public AppUser? User { get; set; }
}
