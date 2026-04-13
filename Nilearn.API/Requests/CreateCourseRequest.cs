namespace Nilearn.API.Requests
{
    public class CreateCourseRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        
        public bool IsPublished { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }
}
