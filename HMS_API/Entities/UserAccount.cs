using System;

namespace HMS_API.Entities;

public class UserAccount
{
    public int Id {get; set;}   
	public required string Username {get; set;}
	public required string UserPassword {get; set;}
	public required string FName {get; set;}
	public required string LName {get; set;}
	public required int UserRole {get; set;}
	public DateOnly Created {get; set;}
	public DateOnly Modified {get; set;}
	public int Deleted {get; set;}

	public ICollection<Enrolment>? Enrolments { get; set; } // = new List<Enrolment>();
	public ICollection<Module>? Modules { get; set; } // = new List<Module>();
	public ICollection<Submission>? Submissions { get; set; } // = new List<Submission>();
	public ICollection<Feedback>? Feedbacks { get; set; } // = new List<Feedback>();
}
