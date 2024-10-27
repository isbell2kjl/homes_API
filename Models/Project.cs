using System.ComponentModel.DataAnnotations;

namespace homes_API.Models;

public class Project
{
    [Required]
    public int ProjectId { get; set; }
    public string? ProjectName { get; set;}
    public string? Description { get; set;}
    public string? MainTitle { get; set; }
    public string? MainText { get; set; }
    public string? Tagline { get; set; }
    public string? LeftTitle { get; set; }
    public string? LeftText { get; set; }
    public string? CenterTitle { get; set; }
    public string? CenterText { get; set; }
    public string? RightTitle { get; set; }
    public string? RightText { get; set; }
    public string? ContactText { get; set; }
    public string? ContactEmail{ get; set; }
    public string? ContactPhone { get; set; }
    public List<User>? Users { get; set; }





}