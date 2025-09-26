namespace  BooksApi.DTOs.Response
{
    public class ResetPasswordDTO
    {
       
        public string Token { get; set; }
        [Required]
        public string OTPNumber { get; set; } = string.Empty;   
        public string ApplicationUserId { get; set; } = string.Empty;   


    }
}
