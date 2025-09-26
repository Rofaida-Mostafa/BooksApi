namespace BooksApi.DTOs.Request
{
    public class BookUpdateRequest
    {
        [Required]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Book title must be between 3 and 150 characters.")]
        public string BookTitle { get; set; } = null!;

        [Required]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Author name must be between 3 and 150 characters.")]
        public string AuthorName { get; set; } = null!;

        public string? Description { get; set; }

        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000.")]
        public decimal? Price { get; set; }

        public int Stock { get; set; } = 0;

        public int? CategoryId { get; set; }

        public int? BookStatus { get; set; }

        [Required(ErrorMessage = "Image is required.")]
        public IFormFile? ImgUrlFile { get; set; }
    }
}
