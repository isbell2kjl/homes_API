public class PendingRequest
{
    public int RequestId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime RequestedAt { get; set; }
    public string Status { get; set; }
} 