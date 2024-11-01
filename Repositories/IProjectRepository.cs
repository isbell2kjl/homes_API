using homes_API.Models;

namespace homes_API.Repositories;

public interface IProjectRepository
{

Project CreateProject(Project newProject);
Project GetProjectById(int projectId);
Project GetProjectFirstRow();
Project? UpdateProject(Project newProject);
void DeleteProjectById(int projectId);
   
}