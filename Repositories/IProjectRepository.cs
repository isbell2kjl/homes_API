using homes_API.Models;

namespace homes_API.Repositories;

public interface IProjectRepository
{
Task<Project> GetPojectAsync(int ProjectId);
Task UpdateProjectAsync(Project project);
   
}