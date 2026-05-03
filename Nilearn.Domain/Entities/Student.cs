namespace Nilearn.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public Guid AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

        public ICollection<Enrollment> Enrollments { get;  private set; } = new List<Enrollment>();

    }
}
