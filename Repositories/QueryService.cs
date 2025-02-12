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

    // public async Task GetQueryResultProperties(int projectId)
    // {
    //     var result = await _context.Posts
    //     .Where(p => p.User.Project.ProjectId == projectId && p.Archive == 0)
    //     .OrderByDescending(p => p.PostId)
    //     .Take(5)
    //     .Select(p => new
    //     {
    //         Combined = p.Title + ", " + p.Content,
    //         p.User.Project.ContactEmail
    //     })
    //     .ToListAsync();

    //     if (!result.Any())
    //     {
    //         Console.WriteLine($"No posts found for ProjectId: {projectId}");
    //         return;
    //     }

    //     // Create CSV content without headers and without ContactEmail
    //     var csvContent = "";
    //     foreach (var item in result)
    //     {
    //         csvContent += $"{item.Combined}\n"; // Only include the Combined field
    //     }

    //     // Create memory stream for attachment
    //     var csvStream = new MemoryStream();
    //     var writer = new StreamWriter(csvStream);
    //     writer.Write(csvContent);
    //     writer.Flush();
    //     csvStream.Position = 0;

    //     var contactEmail = result.First().ContactEmail;
    //     var subject = "Weekly Report: Latest Posts";

    //     // Generate filename based on the current date
    //     var currentDate = DateTime.Now.ToString("yyyy_MM_dd");
    //     var fileName = $"{currentDate}_properties.csv";

    //     // Send email with CSV attachment
    //     _emailRepository.SendWithAttachment(contactEmail, subject, "Please find the weekly report attached.", csvStream, fileName);

    //     Console.WriteLine($"Email sent to {contactEmail} with CSV attachment.");
    // }

    // public async Task GetQueryResultTasks(int projectId)
    // {
    //     // Query to get Title and Text with the specified conditions
    //     var result = await _context.Posts
    //        .Where(p => p.User.Project.ProjectId == projectId && p.Archive == 0)
    //         .SelectMany(p => p.Comments.Select(c => new
    //         {
    //             p.Title,
    //             c.Text
    //         }))
    //         .ToListAsync();

    //     // Create CSV content without headers
    //     var csvContent = "";
    //     foreach (var item in result)
    //     {
    //         csvContent += $"{item.Title}, {item.Text}\n";
    //     }

    //     // Create memory stream for attachment
    //     using var csvStream = new MemoryStream();
    //     using var writer = new StreamWriter(csvStream);
    //     writer.Write(csvContent);
    //     writer.Flush();
    //     csvStream.Position = 0;

    //     // Generate filename based on current date
    //     var currentDate = DateTime.Now.ToString("yyyy_MM_dd");
    //     var fileName = $"{currentDate}_actions.csv";

    //     // Get contact email from the related project
    //     var contactEmail = await _context.Projects
    //         .Where(p => p.ProjectId == projectId)
    //         .Select(p => p.ContactEmail)
    //         .FirstOrDefaultAsync();

    //     if (string.IsNullOrEmpty(contactEmail))
    //         throw new Exception("No contact email found for the specified project.");

    //     // Subject for the email
    //     var subject = "Weekly Report: Post Details";

    //     // Send email with CSV attachment
    //     _emailRepository.SendWithAttachment(contactEmail, subject, "Please find your weekly post actions report attached.", csvStream, fileName);
    // }

    public async Task QueryAndSendReports(int projectId)
    {
        // Generate "properties" report
        var propertiesResult = await _context.Posts
            .Where(p => p.User.Project.ProjectId == projectId && p.Archive == 0)
            .OrderByDescending(p => p.PostId)
            .Select(p => new { Combined = $"{p.Title}, {p.Content}" })
            .ToListAsync();

        var propertiesCsvContent = string.Join("\n", propertiesResult.Select(r => r.Combined));
        var propertiesStream = new MemoryStream();
        using (var writer = new StreamWriter(propertiesStream, leaveOpen: true))
        {
            writer.Write(propertiesCsvContent);
            writer.Flush();
            propertiesStream.Position = 0;
        }

        // Generate "actions" report
        var actionsResult = await _context.Posts
            .Where(p => p.User.Project.ProjectId == projectId && p.Archive == 0)
            .OrderByDescending(p => p.PostId)
            .SelectMany(p => p.Comments.Select(c => new { p.Title, c.Text }))
            .ToListAsync();

        var actionsCsvContent = string.Join("\n", actionsResult.Select(r => $"{r.Title}, {r.Text}"));
        var actionsStream = new MemoryStream();
        using (var writer = new StreamWriter(actionsStream, leaveOpen: true))
        {
            writer.Write(actionsCsvContent);
            writer.Flush();
            actionsStream.Position = 0;
        }

        // Generate filenames
        var currentDate = DateTime.Now.ToString("yyyy_MM_dd");
        var propertiesFileName = $"{currentDate}_properties.csv";
        var actionsFileName = $"{currentDate}_actions.csv";

        // Get contact email
        var contactEmail = await _context.Projects
            .Where(p => p.ProjectId == projectId)
            .Select(p => p.ContactEmail)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(contactEmail))
            throw new Exception("No contact email found for the specified project.");

        var subject = "Weekly Reports: Properties and Actions";

        // Create attachment list
        var attachments = new List<(Stream, string)>
    {
        (propertiesStream, propertiesFileName),
        (actionsStream, actionsFileName)
    };

        // Send email with both attachments
        _emailRepository.SendWithMultipleAttachments(contactEmail, subject, "Please find your weekly reports attached.", attachments);
    }

}

