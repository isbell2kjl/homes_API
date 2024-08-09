using blog_API.Models;

namespace blog_API.Repositories;

public interface IContactRepository
{
    void SendContact(Contact model, string origin);

}