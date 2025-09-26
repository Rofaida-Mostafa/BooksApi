using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace  BooksApi.Models;

public partial class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MinLength(3, ErrorMessage = "Name must contain at least 3 letters.")]
    [MaxLength(100)]
    [RegularExpression("^[\u0621-\u064A a-zA-Z]+$", ErrorMessage = "Name must contain only letters.")]
    public string Name { get; set; } = null!;

    [MaxLength(200)]
    public string? Description { get; set; }

    public bool Status { get; set; } = true;

    [InverseProperty("Category")]
   // [JsonIgnore]
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
