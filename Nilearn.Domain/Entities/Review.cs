namespace Nilearn.Domain.Entities;

public class Review : BaseEntity
{
    public int CourseId { get; private set; }
    public Course? Course { get; private set; }
    public int StudentId { get; private set; }
    public Student? Student { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; }

    public Review(int courseId, int studentId, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5");

        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Comment cannot be empty");

        CourseId = courseId;
        StudentId = studentId;
        Rating = rating;
        Comment = comment;
    }

    public void Update(int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5");

        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Comment cannot be empty");

        Rating = rating;
        Comment = comment;
    }

    

}
