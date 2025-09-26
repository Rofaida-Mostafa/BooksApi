namespace BooksApi.DTOs.Request
{
    public class BookFilterRequest
    {
        public string? BookName { get; set; }
        public double? Price { get; set; }
        public int? CategoryId { get; set; }
    }
}
