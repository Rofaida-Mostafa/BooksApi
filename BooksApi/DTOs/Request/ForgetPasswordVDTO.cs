﻿
namespace  BooksApi.DTOs.Request
{
    public class ForgetPasswordDTO
    {
      

        [Required]
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
