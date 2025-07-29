namespace BlazorMauiApp1.Models
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = "";
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; } = "";
        public string SmtpPass { get; set; } = "";
        public string SenderEmail { get; set; } = "";
        public string SenderName { get; set; } = "";
    }
}
