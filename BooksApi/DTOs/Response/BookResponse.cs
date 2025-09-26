using System.ComponentModel.DataAnnotations.Schema;

namespace BooksApi.DTOs.Response
{
    public class BookResponse
    {
        public int Id { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string BookTitle { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? ImgUrl { get; set; }     
        public DateTime CreatedAt { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryDescription { get; set; }
    }
}
