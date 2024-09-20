using homes_API.Models;

namespace homes_API.Repositories;

public interface IContactRepository
{
    void SendContact(Contact model, string origin);

}