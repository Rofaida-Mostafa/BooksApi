namespace  BooksApi.DTOs.Response
{
    public class NotificationDTO
    {
        public string Msg { get; set; } = string.Empty;
        public string TraceId { get; set; }
        public DateTime CreatedAT { get; set; }
    }
}
