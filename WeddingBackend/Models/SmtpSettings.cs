namespace WeddingBackend.Models
{
    public class SmtpSettings
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string User { get; set; } = "";
        public string Password { get; set; } = ""; // keep this outside source control (user-secrets or env)
        public string To { get; set; } = ""; // recipient email (the couple)
        public string FromAddress { get; set; } = ""; // optional from address if you want a fixed sender
    }
}