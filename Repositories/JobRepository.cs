using homes_API.Migrations;
using homes_API.Models;

namespace homes_API.Repositories;

public class JobRepository : IJobRepository
{
    private readonly PostDbContext _context;

    public JobRepository(PostDbContext context)
    { 
        _context = context;
    }
    public Job CreateJob(Job newJob)
    {
        _context.Jobs!.Add(newJob);
        _context.SaveChanges();
        return newJob;

    }

    public void DeleteJobById(int jobId)
    {
        var job = _context.Jobs!.Find(jobId);
        if (job != null)
        {
            _context.Jobs.Remove(job);
            _context.SaveChanges();
        }
    }

    public IEnumerable<Job> GetAllJobs()
    {
        return _context.Jobs!.ToList();
    }

    public Job? GetJobById(int jobId)
    {
        return _context.Jobs!.SingleOrDefault(t => t.JobId == jobId);
    }

    public Job? UpdateJob(Job newJob)
    {
        var originalJob = _context.Jobs!.Find(newJob.JobId);
        if (originalJob != null)
        {
            originalJob.Title = newJob.Title;
            originalJob.Completed = newJob.Completed;
            _context.SaveChanges();
        }
        return originalJob;
    }
}