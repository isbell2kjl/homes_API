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
            // Update only non-null or non-empty fields
            if (!string.IsNullOrEmpty(newProject.ProjectName))
                originalProject.ProjectName = newProject.ProjectName;

            if (!string.IsNullOrEmpty(newProject.ContactEmail))
                originalProject.ContactEmail = newProject.ContactEmail;

            // Optional fields - update if provided
            if (!string.IsNullOrEmpty(newProject.SiteName))
                originalProject.SiteName = newProject.SiteName;

            if (!string.IsNullOrEmpty(newProject.MainTitle))
                originalProject.MainTitle = newProject.MainTitle;

            if (!string.IsNullOrEmpty(newProject.MainText))
                originalProject.MainText = newProject.MainText;

            if (!string.IsNullOrEmpty(newProject.Tagline))
                originalProject.Tagline = newProject.Tagline;

            if (!string.IsNullOrEmpty(newProject.LeftTitle))
                originalProject.LeftTitle = newProject.LeftTitle;

            if (!string.IsNullOrEmpty(newProject.LeftText))
                originalProject.LeftText = newProject.LeftText;

            if (!string.IsNullOrEmpty(newProject.CenterTitle))
                originalProject.CenterTitle = newProject.CenterTitle;

            if (!string.IsNullOrEmpty(newProject.CenterText))
                originalProject.CenterText = newProject.CenterText;

            if (!string.IsNullOrEmpty(newProject.RightTitle))
                originalProject.RightTitle = newProject.RightTitle;

            if (!string.IsNullOrEmpty(newProject.RightText))
                originalProject.RightText = newProject.RightText;

            if (!string.IsNullOrEmpty(newProject.ContactText))
                originalProject.ContactText = newProject.ContactText;

            if (!string.IsNullOrEmpty(newProject.ContactPhone))
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

    public Project? GetProjectFirstRow()
    {
        return _context.Projects
            .Where(p => p.ProjectName != null && p.ContactEmail != null) // Only load valid rows
            .FirstOrDefault();
    }


    public async Task<(bool Success, string Message)> RequestToJoinProject(int userId, string projectEmail)
    {
        // Find project by email
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.ContactEmail == projectEmail);
        if (project == null)
            return (false, "Project not found.");

        // Check if a pending request already exists
        var existingRequest = await _context.Requests
            .FirstOrDefaultAsync(r => r.UserId == userId && r.ProjectId == project.ProjectId && r.Status == "Pending");
        if (existingRequest != null)
            return (false, "Request already pending.");

        // Create join request
        var joinRequest = new Request
        {
            UserId = userId,
            ProjectId = project.ProjectId,
            Status = "Pending"
        };
        _context.Requests.Add(joinRequest);
        await _context.SaveChangesAsync();

        return (true, "Join request submitted.");
    }

    public async Task<List<PendingRequest>> GetPendingRequests(int adminProjectId)
    {
        {
            var pendingRequests = await _context.Requests
                .Where(r => r.ProjectId == adminProjectId && r.Status == "Pending")
                .Include(r => r.User)  // Ensure User navigation property is loaded
                .Select(r => new PendingRequest
                {
                    RequestId = r.RequestId,
                    UserId = r.UserId,
                    UserName = r.User.UserName,
                    Email = r.User.Email,
                    RequestedAt = r.RequestedAt,
                    Status = r.Status
                })
                .ToListAsync();

            return pendingRequests;
        }
    }

    public async Task<List<PendingRequest>> GetUserRequests(int userId)
    {
        {
            var pendingRequests = await _context.Requests
                .Where(r => r.UserId == userId && r.Status == "Pending")
                .Include(r => r.User)  // Ensure User navigation property is loaded
                .Select(r => new PendingRequest
                {
                    RequestId = r.RequestId,
                    UserId = r.UserId,
                    UserName = r.User.UserName,
                    Email = r.User.Email,
                    RequestedAt = r.RequestedAt,
                    Status = r.Status
                })
                .ToListAsync();

            return pendingRequests;
        }
    }

    public async Task<bool> EmailExists(string email)
    {
        //Check if a project email exists and that has an ID greater than 1.
        //A user who wants to join a group/project must join a group with an existing email address
        //and must have a ProjectID >1.
        return await _context.Projects.AnyAsync(u => u.ContactEmail == email && u.ProjectId > 1);

    }

    public async Task<(bool Success, string Message)> RejectJoinRequest(int requestId)
    {
        var joinRequest = await _context.Requests.FindAsync(requestId);
        if (joinRequest == null)
        {
            return (false, "Join request not found.");
        }

        joinRequest.Status = "Rejected";
        _context.Requests.Update(joinRequest);
        await _context.SaveChangesAsync();

        return (true, "Join request rejected successfully.");
    }


    public async Task<(bool Success, string Message)> ApproveJoinRequest(int requestId)
    {
        // Find the join request in the database
        var joinRequest = await _context.Requests.FindAsync(requestId);

        if (joinRequest == null)
        {
            return (false, "Join request not found.");
        }

        // Check if the request is already approved or rejected
        if (joinRequest.Status == "Approved")
        {
            return (false, "Request is already approved.");
        }

        if (joinRequest.Status == "Rejected")
        {
            return (false, "Request has been rejected.");
        }

        // Update the request status to 'Approved'
        joinRequest.Status = "Approved";

        // Assign the user to the project
        var user = await _context.Users.FindAsync(joinRequest.UserId);
        if (user != null)
        {
            user.ProjId_fk = joinRequest.ProjectId;  // Set foreign key
        }

        // Save changes to the database
        await _context.SaveChangesAsync();

        return (true, "Join request approved successfully.");
    }
}