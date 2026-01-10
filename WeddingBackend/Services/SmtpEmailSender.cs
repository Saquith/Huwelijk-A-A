using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using WeddingBackend.Models;

namespace WeddingBackend.Services
{
    public class SmtpEmailSender
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<SmtpSettings> options, ILogger<SmtpEmailSender> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(Models.ContactRequest req)
        {
            var message = new MimeMessage();

            try
            {
                // From: prefer configured FromAddress; otherwise use requestor's email as display
                if (!string.IsNullOrWhiteSpace(_settings.FromAddress))
                {
                    message.From.Add(MailboxAddress.Parse(_settings.FromAddress));
                    message.ReplyTo.Add(new MailboxAddress(req.Name, req.Email));
                }
                else
                {
                    message.From.Add(new MailboxAddress(req.Name, req.Email));
                }

                if (string.IsNullOrWhiteSpace(_settings.To))
                    throw new InvalidOperationException("Smtp 'To' recipient is not configured.");

                message.To.Add(MailboxAddress.Parse(_settings.To));
                message.Subject = string.IsNullOrWhiteSpace(req.Subject) ? "Wedding site message" : req.Subject;

                var bodyText =
$@"You received a message from the wedding site:

Name: {req.Name}
Email: {req.Email}
Subject: {req.Subject}

Message:
{req.Message}
";

                var builder = new BodyBuilder { TextBody = bodyText };
                message.Body = builder.ToMessageBody();

                using var client = new MailKit.Net.Smtp.SmtpClient();
                // Accept all SSL certificates (useful for dev). Remove or tighten in production if required.
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                var secureSocket = _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

                _logger.LogInformation("Connecting to SMTP {Host}:{Port} (SSL={UseSsl})", _settings.Host, _settings.Port, _settings.UseSsl);
                await client.ConnectAsync(_settings.Host, _settings.Port, secureSocket);

                if (!string.IsNullOrWhiteSpace(_settings.User))
                {
                    _logger.LogDebug("Authenticating SMTP user {User}", _settings.User);
                    await client.AuthenticateAsync(_settings.User, _settings.Password);
                }

                _logger.LogInformation("Sending email to {To}", _settings.To);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                _logger.LogInformation("Email sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending email");
                throw;
            }
        }
    }
}