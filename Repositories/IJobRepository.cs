using homes_API.Models;
namespace homes_API.Repositories;

public interface IJobRepository {

    IEnumerable<Job> GetAllJobs();
    Job? GetJobById(int jobId);
    Job CreateJob(Job newJob);
    Job? UpdateJob(Job newJob);
    void DeleteJobById(int jobId);
}