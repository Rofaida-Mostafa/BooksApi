namespace  BooksApi.DTOs.Response
{
    public class NewPasswordDTO
    {
     
        public string ApplicationUserId { get; set; } = string.Empty;
        public string Token { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required, DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
