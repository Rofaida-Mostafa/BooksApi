using Microsoft.AspNetCore.Authorization;

namespace  BooksApi.Models
{
   
    [PrimaryKey(nameof(ApplicationUserId), nameof(BookId))]
    public class Cart
    {
            public string ApplicationUserId { get; set; }
            public ApplicationUser ApplicationUser { get; set; }
            public int BookId { get; set; }
            public Book Book { get; set; }
            public int Count { get; set; }
        

    }
}
