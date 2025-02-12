namespace homes_API.Repositories;

public interface IEmailRepository
{

   void Send(string? to, string subject, string html, string? from = null);
   
   // void SendWithAttachment(string to, string subject, string html, Stream attachmentStream,
   //    string fileName = null, string from = null);
   void SendWithMultipleAttachments(string to, string subject, string html,
    List<(Stream Stream, string FileName)> attachments, string from = null);

}