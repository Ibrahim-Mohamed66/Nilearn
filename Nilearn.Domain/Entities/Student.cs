namespace Nilearn.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public Guid AppUserId { get; set; }
        public string StudentNumber { get; set; } = string.Empty;
        public int CurrentLevel { get; set; } // Year 1, 2, 3, 4
        public AppUser? AppUser { get; set; }
       
    }
}
