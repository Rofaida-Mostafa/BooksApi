using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace  BooksApi.Models;

public partial class Book
{
    [Key]
    public int Id { get; set; }

    [StringLength(150)]
    [Required]
    [MinLength(3, ErrorMessage = "Name must contain at least 3 letters.")]
    [MaxLength(100)]
    [RegularExpression("^[\u0621-\u064A a-zA-Z]+$", ErrorMessage = "Name must contain only letters.")]

   
    public string AuthorName { get; set; } = string.Empty;

    public string BookTitle { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Price { get; set; }

    [StringLength(255)]
    public string ImgUrl { get; set; } = string.Empty;

    public string? ASIN { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? BookStatus { get; set; }

    public int? CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Books")]

    public virtual Category? Category { get; set; }


}
