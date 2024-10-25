using homes_API.Migrations;
using homes_API.Models;
using Microsoft.EntityFrameworkCore;

namespace homes_API.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly PostDbContext _context;

    public ProjectRepository(PostDbContext context)
    {
        _context = context;
    }

    public async Task<Project> GetPojectAsync(int projectId)
    {
        return await _context.Project.FindAsync(projectId);
    }

    public async Task UpdateProjectAsync(Project project)
    {
        _context.Project!.Update(project);
        await _context.SaveChangesAsync();
    }
}