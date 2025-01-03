using homes_API.Models;

namespace homes_API.Repositories;

public interface IProjectRepository
{

Project CreateProject(Project newProject);
Project GetProjectById(int projectId);
Project GetProjectFirstRow();
Project? UpdateProject(Project newProject);
Task<bool> EmailExists(string email);
Task<(bool Success, string Message)> RequestToJoinProject(int userId, string projectEmail);
Task<List<PendingRequest>> GetPendingRequests(int adminProjectId);
Task<List<PendingRequest>> GetUserRequests(int userId);
Task<(bool Success, string Message)> ApproveJoinRequest(int requestId);
Task<(bool Success, string Message)> RejectJoinRequest(int requestId);
void DeleteProjectById(int projectId);
   
}