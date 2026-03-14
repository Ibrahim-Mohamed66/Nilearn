using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Enums;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IConfiguration _configuration;

        public DbInitializer(
            AppDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task InitializeAsync()
        {
            // Apply pending migrations
            if (_context.Database.GetPendingMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }

            // Create roles
            foreach (var role in Enum.GetNames(typeof(Role)))
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });
                }
            }

            // Read admin credentials from configuration
            var adminEmail = _configuration["SuperAdmin:Email"];
            var adminPassword = _configuration["SuperAdmin:Password"];

            // Check if admin already exists
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User",
                    PhoneNumber = "01234567891",
                    PhoneNumberConfirmed = true,
                    DateOfBirth = new DateOnly(2005, 6, 6)
                };

                var result = await _userManager.CreateAsync(newAdmin, adminPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newAdmin, Role.SuperAdmin.ToString());
                }
                else
                {
                    throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}