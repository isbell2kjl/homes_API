using homes_API.Models;

namespace homes_API.Repositories;

public interface IProjectRepository
{

Project CreateProject(Project newProject);
Project GetProjectById(int projectId);
Task<Project> GetPojectAsync(int projectId);
Task UpdateProjectAsync(Project project);
   
}