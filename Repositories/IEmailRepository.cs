namespace blog_API.Repositories;

public interface IEmailRepository {

   void Send(string to, string subject, string html, string from = null); 
}