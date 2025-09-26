namespace  BooksApi.Models
{
    public class UserOTP
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [MaxLength(6)]
        public string OTPNumber { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }
    }
}
