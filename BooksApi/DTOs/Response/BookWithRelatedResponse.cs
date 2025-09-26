namespace BooksApi.DTOs.Response
{
    public class BookWithRelatedResponse
    {
        public Book book { get; set; } = null!;
        public List<Book> RelatedBooks { get; set; } = null!;
        public List<Book> TopTraffic { get; set; } = null!;
        public List<Book> SimilarBooks { get; set; } = null!;
    }
}
