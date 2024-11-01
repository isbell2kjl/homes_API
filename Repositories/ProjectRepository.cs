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

public Project? UpdateProject(Project newProject)
    {
        var originalProject = _context.Projects!.Find(newProject.ProjectId);
        if (originalProject != null)
        {
            originalProject.SiteName = newProject.SiteName;
            originalProject.ProjectName = newProject.ProjectName;
            originalProject.MainTitle = newProject.MainTitle;
            originalProject.MainText = newProject.MainText;
            originalProject.Tagline = newProject.Tagline;
            originalProject.LeftTitle = newProject.LeftTitle;
            originalProject.LeftText = newProject.LeftText;
            originalProject.CenterTitle = newProject.CenterTitle;
            originalProject.CenterText = newProject.CenterText;
            originalProject.RightTitle = newProject.RightTitle;
            originalProject.RightText = newProject.RightText;
            originalProject.ContactText = newProject.ContactText;
            originalProject.ContactEmail = newProject.ContactEmail;
            originalProject.ContactPhone = newProject.ContactPhone;
            _context.SaveChanges();
        }
        return originalProject;
    }

    public Project GetProjectById(int projectId)
    {
        return _context.Projects!.FirstOrDefault(p => p.ProjectId == projectId) ?? new Project();
        
    }

    public void DeleteProjectById(int projectId)
    {
        var project = _context.Projects!.Find(projectId);
        if (project != null)
        {
            _context.Projects.Remove(project);
            _context.SaveChanges();
        }
    }

    public Project GetProjectFirstRow()
    {
        return _context.Projects.FirstOrDefault();
    }
}