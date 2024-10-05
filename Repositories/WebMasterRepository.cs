using homes_API.Models;
using homes_API.Helpers;

namespace homes_API.Repositories;

public class WebMasterRepository : IWebMasterRepository
{

    private readonly IEmailRepository _emailRepository;
    private static IConfiguration? _config;

    public WebMasterRepository(IEmailRepository emailRepository, IConfiguration config)
    {
        _emailRepository = emailRepository;
        _config = config;
    }

    public void SendWebMaster(Contact model, string origin)
    {
        var user = model;

        // send email
        sendContactEmail(user, origin);
    }

    private void sendContactEmail(Contact user, string origin)
    {
        var webmaster = _config.GetValue<String>("WebMaster");
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
            to: webmaster,
            subject: "WebSite Request",
            html: $@"<h4>This message is from:</h4>
                        {message}"
        );
    }

}

