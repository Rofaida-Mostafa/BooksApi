namespace  BooksApi.Models
{
    public class Promotion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        [Range(0, 100)]
        public double DiscountPercentage { get; set; }

        public bool Status { get; set; }

        public int TotalUsed { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
