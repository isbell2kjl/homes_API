using homes_API.Models;

namespace homes_API.Repositories;

public interface IWebMasterRepository
{
    void SendWebMaster(Contact model, string origin);

}