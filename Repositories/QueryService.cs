using System.Text;
using homes_API.Migrations;
using Microsoft.EntityFrameworkCore;


namespace homes_API.Repositories;

public class QueryService : IQueryService
{

    private readonly PostDbContext _context;
    private readonly IEmailRepository _emailRepository;
    private readonly IConfiguration _config;

    public QueryService(PostDbContext context, IEmailRepository emailRepository, IConfiguration config)
    {
        _context = context;
        _emailRepository = emailRepository;
        _config = config;

    }

    public async Task GetQueryResultProperties(int projectId)
    {
        var result = await _context.Posts
        .Where(p => p.User.Project.ProjectId == projectId && p.Archive == 0)
        .OrderByDescending(p => p.PostId)
        .Take(5)
        .Select(p => new
        {
            Combined = p.Title + ", " + p.Content,
            p.User.Project.ContactEmail
        })
        .ToListAsync();

        if (!result.Any())
        {
            Console.WriteLine($"No posts found for ProjectId: {projectId}");
            return;
        }

        // Create CSV content without headers and without ContactEmail
        var csvContent = "";
        foreach (var item in result)
        {
            csvContent += $"{item.Combined}\n"; // Only include the Combined field
        }

        // Create memory stream for attachment
        var csvStream = new MemoryStream();
        var writer = new StreamWriter(csvStream);
        writer.Write(csvContent);
        writer.Flush();
        csvStream.Position = 0;

        var contactEmail = result.First().ContactEmail;
        var subject = "Weekly Report: Latest Posts";

        // Generate filename based on the current date
        var currentDate = DateTime.Now.ToString("yyyy_MM_dd");
        var fileName = $"{currentDate}_properties.csv";

        // Send email with CSV attachment
        _emailRepository.SendWithAttachment(contactEmail, subject, "Please find the weekly report attached.", csvStream, fileName);

        Console.WriteLine($"Email sent to {contactEmail} with CSV attachment.");
    }

    public async Task GetQueryResultTasks(int projectId)
    {
        // Query to get Title and Text with the specified conditions
        var result = await _context.Posts
           .Where(p => p.User.Project.ProjectId == projectId && p.Archive == 0)
            .SelectMany(p => p.Comments.Select(c => new
            {
                p.Title,
                c.Text
            }))
            .ToListAsync();

        // Create CSV content without headers
        var csvContent = "";
        foreach (var item in result)
        {
            csvContent += $"{item.Title}, {item.Text}\n";
        }

        // Create memory stream for attachment
        using var csvStream = new MemoryStream();
        using var writer = new StreamWriter(csvStream);
        writer.Write(csvContent);
        writer.Flush();
        csvStream.Position = 0;

        // Generate filename based on current date
        var currentDate = DateTime.Now.ToString("yyyy_MM_dd");
        var fileName = $"{currentDate}_tasks.csv";

        // Get contact email from the related project
        var contactEmail = await _context.Projects
            .Where(p => p.ProjectId == projectId)
            .Select(p => p.ContactEmail)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(contactEmail))
            throw new Exception("No contact email found for the specified project.");

        // Subject for the email
        var subject = "Weekly Report: Post Details";

        // Send email with CSV attachment
        _emailRepository.SendWithAttachment(contactEmail, subject, "Please find the weekly post details report attached.", csvStream, fileName);
    }
}
