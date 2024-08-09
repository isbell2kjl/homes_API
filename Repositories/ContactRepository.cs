using blog_API.Models;
using blog_API.Helpers;

namespace blog_API.Repositories;

public class ContactRepository : IContactRepository
{

    private readonly IEmailRepository _emailRepository;

    public ContactRepository(IEmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
    }

    public void SendContact(Contact model, string origin)
    {
        var user = model;

        // send email
        sendContactEmail(user, origin);
    }

    private void sendContactEmail(Contact user, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))

        {
            // origin exists if request sent from browser single page app (e.g. Angular or React)
            // so send link to verify via single page app
            var resetUrl = $"{origin}/contact";
            message = $@"<p>Name: {user.Name}<br><br>Email: {user.Email}<br><br>Phone: {user.Phone}</p>
                            <p>{user.Message}</p>";
        }
        else
        {
            message = $@"<p>{user.Name}<br>{user.Email}<br>{user.Phone}</p>
                            <p>{user.Message}</p>";
        }

        _emailRepository.Send(
            to: "myadvantagep@gmail.com" ,
            subject: "Contact Request",
            html: $@"<h4>This message is from:</h4>
                        {message}"
        );
    }

}

