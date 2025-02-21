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

    public async Task QueryAndSendReports(int projectId)
    { 
       var propertiesResult = await _context.Posts
            .Where(p => p.User.ProjId_fk == projectId && p.Archive == 0)
            .OrderByDescending(p => p.PostId)
            .Select(p => new { p.Title, p.Content }) // Fetch data only
            .ToListAsync();

        // Move string concatenation to C#
        var formattedProperties = propertiesResult
            .Select(p => $"{p.Title}, {p.Content}")
            .ToList();

        var propertiesCsvContent = string.Join("\n", propertiesResult.Select(r => $"{r.Title}, {r.Content}"));

        var propertiesStream = new MemoryStream();
        using (var writer = new StreamWriter(propertiesStream, leaveOpen: true))
        {
            writer.Write(propertiesCsvContent);
            writer.Flush();
            propertiesStream.Position = 0;
        }

        // Generate "actions" report

        var actionsResult = await _context.Comments
            .Where(c => c.Post.User.Project.ProjectId == projectId && c.Post.Archive == 0)
            .OrderByDescending(c => c.Post.PostId)
            .Select(c => new { c.Post.Title, c.Text })
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


        var project = await _context.Projects.FindAsync(projectId);
        var contactEmail = project?.ContactEmail;

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

