namespace homes_API.Repositories;


public interface IQueryService
{
// Task GetQueryResultProperties(int projectId);
// Task GetQueryResultTasks(int projectId);
Task QueryAndSendReports(int projectId);

}