using homes_API.Migrations;
using homes_API.Models;

namespace homes_API.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly PostDbContext _context;

    public ProjectRepository(PostDbContext context)
    {
        _context = context;
    }

    public Project CreateProject(Project newProject)
    {
        _context.Projects!.Add(newProject);
        _context.SaveChanges();
        return newProject;
    }

    public async Task<Project> GetPojectAsync(int projectId)
    {
        return await _context.Projects.FindAsync(projectId);
    }

    public Project GetPojectById(int projectID)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateProjectAsync(Project project)
    {
        _context.Projects!.Update(project);
        await _context.SaveChangesAsync();
    }

    public Project GetProjectById(int projectId)
    {
        return _context.Projects!.FirstOrDefault(p => projectId == projectId)!;
        
    }
}