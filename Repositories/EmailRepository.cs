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

    public void SendWithAttachment(string to, string subject, string html, Stream attachmentStream, string fileName = "", string from = "")
    {
        var attachments = new List<(Stream, string)>();

        // Add attachment if provided
        if (attachmentStream != null)
        {
            attachments.Add((attachmentStream, fileName));
        }

        // Call common method with list of attachments
        SendEmail(to, subject, html, from, attachments);
    }

    public void SendWithMultipleAttachments(string to, string subject, string html, List<(Stream Stream, string FileName)> attachments, string from = "")
    {
        // Call the common method directly for multiple attachments
        SendEmail(to, subject, html, from, attachments);
    }

    private void SendEmail(string to, string subject, string html, string from, List<(Stream Stream, string FileName)> attachments = null)
    {
        // Create message
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(from ?? _appSettings.EmailFrom));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        // Email body
        var body = new TextPart(TextFormat.Html) { Text = html };
        var multipart = new Multipart("mixed") { body };

        // Attach files if attachments are provided
        if (attachments != null && attachments.Count > 0)
        {
            foreach (var (stream, fileName) in attachments)
            {
                var attachment = new MimePart("text", "csv")
                {
                    Content = new MimeContent(stream),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = fileName
                };
                multipart.Add(attachment);
            }
        }

        // Set the email body
        email.Body = multipart;

        // Send email
        using var smtp = new SmtpClient();
        smtp.Connect(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.StartTls);
        smtp.Authenticate(_appSettings.SmtpUser, _appSettings.SmtpPass);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}
