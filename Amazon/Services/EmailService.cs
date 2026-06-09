using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Amazon.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration   _config;
    private readonly ILogger<EmailService> _logger;
    private readonly IWebHostEnvironment   _env;

    public EmailService(IConfiguration config,
                        ILogger<EmailService> logger,
                        IWebHostEnvironment env)
    {
        _config = config;
        _logger = logger;
        _env    = env;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var section = _config.GetSection("Email");
        var username = section["Username"] ?? "";
        var password = section["Password"] ?? "";
        var fromAddr = section["FromAddress"] ?? username;
        var fromName = section["FromName"] ?? "Amazon Clone";

        // Dev mode without SMTP config — just log
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning(
                "[EmailService DEV] To: {To} | Subject: {Subject}\n{Body}",
                toEmail, subject, htmlBody);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromAddr));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        var host    = section["SmtpHost"] ?? "smtp.gmail.com";
        var port    = int.Parse(section["SmtpPort"] ?? "587");
        var useSsl  = bool.Parse(section["EnableSsl"] ?? "true");

        await client.ConnectAsync(host, port, useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
        await client.AuthenticateAsync(username, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
