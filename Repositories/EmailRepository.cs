using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using homes_API.Helpers;

namespace homes_API.Repositories;


public class EmailRepository : IEmailRepository
{
    private readonly AppSettings _appSettings;

    public EmailRepository(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public void Send(string to, string subject, string html, string from = "")
    {
        // Call the common method without attachment
        SendEmail(to, subject, html, from);
    }

    public void SendWithAttachment(string to, string subject, string html, Stream attachmentStream, string fileName = "Report.csv", string from = "")
    {
        // Call the common method with attachment
        SendEmail(to, subject, html, from, attachmentStream, fileName);
    }

    private void SendEmail(string to, string subject, string html, string from = "", Stream attachmentStream = null, string fileName = "Report.csv")
    {
        // Create message
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from ?? _appSettings.EmailFrom));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        // Email body
        var body = new TextPart(TextFormat.Html) { Text = html };
        var multipart = new Multipart("mixed") { body };

        // Attach CSV if stream is provided
        if (attachmentStream != null)
        {
            var attachment = new MimePart("text", "csv")
            {
                Content = new MimeContent(attachmentStream),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = fileName
            };
            multipart.Add(attachment);
        }

        // Send email with or without attachment
        email.Body = multipart;

        using var smtp = new SmtpClient();
        smtp.Connect(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.StartTls);
        smtp.Authenticate(_appSettings.SmtpUser, _appSettings.SmtpPass);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}
