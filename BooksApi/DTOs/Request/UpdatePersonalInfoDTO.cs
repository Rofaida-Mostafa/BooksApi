namespace  BooksApi.DTOs.Request
{
    public class UpdatePersonalInfoDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
 
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zipcode { get; set; }
        public string? ProfileImage { get; set; }

    }
}
