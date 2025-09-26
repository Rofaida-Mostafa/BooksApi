namespace BooksApi.DTOs.Request
{
    public class CategoriesRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
    }
}
