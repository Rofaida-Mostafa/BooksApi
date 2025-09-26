
namespace  BooksApi.DTOs.Response
{
    public class ResendEmailConfirmationDTO
    {
     

        [Required]
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
