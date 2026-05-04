using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Infrastructure.Configuration;
using Nilearn.Infrastructure.Configurations;
using Npgsql.EntityFrameworkCore.PostgreSQL;




namespace Nilearn.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<InstructorWallet> InstructorWallets { get; set; }
        public DbSet<PlatformWallet> PlatformWallets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new AppUserConfiguration());
            builder.ApplyConfiguration(new StudentConfiguration());
            builder.ApplyConfiguration(new InstructorConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new CourseConfiguration());
            builder.ApplyConfiguration(new SectionConfiguration());
            builder.ApplyConfiguration(new LessonConfiguration());
            builder.ApplyConfiguration(new EnrollmentConfiguration());
            builder.ApplyConfiguration(new PaymentConfiguration());
            builder.ApplyConfiguration(new WalletTransactionConfiguration());
            builder.ApplyConfiguration(new InstructorWalletConfiguration());
            builder.ApplyConfiguration(new PlatformWalletConfiguration());

            // Global query filter for soft delete
            builder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Course>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Section>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Lesson>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Enrollment>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Payment>().HasQueryFilter(e => !e.IsDeleted);

            





        }
    }
}
