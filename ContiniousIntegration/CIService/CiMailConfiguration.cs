namespace CIService
{
    public class CiMailConfiguration
    {
        public string Sender { get; set; }
        public string Password { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpHost { get; set; }
        public string Receiver { get; set; }
    }
}