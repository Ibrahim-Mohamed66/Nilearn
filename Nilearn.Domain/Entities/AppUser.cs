using Microsoft.AspNetCore.Identity;
namespace Nilearn.Domain.Entities;
public class AppUser : IdentityUser<Guid>
{

    // ===== Personal Info =====
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string? ProfileImageUrl { get; set; }
    
    public DateOnly? DateOfBirth { get; set; }

    // ===== System Info =====
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false; // Soft delete
  
    public string? FullName => $"{FirstName} {LastName}";
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public Instructor? InstructorProfile { get; set; }
    public Student? StudentProfile { get; set; }
    public void RevokeActiveRefreshTokens()
    {
        foreach (var token in RefreshTokens.Where(t => t.IsActive))
           token.Revoke();
    }

}
