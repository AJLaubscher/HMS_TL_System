using System;

namespace HMS_API.Entities;

public class Submission
{
	public int Id {get; set;}
	public required int StudID {get; set;}                          // get from user/login
    public UserAccount? UserAccount {get; set;}                     // call on class
	public required int AssignID {get; set;}                        // get from chosen assignment
    public Assignment? Assignment {get; set;}                       // call on class
	public DateOnly SubDate {get; set;}
	public required string FilePath {get; set;}                     // construct filepath to exact submission
	public bool Marked {get; set;}
	public DateOnly Created {get; set;}
	public DateOnly Modified {get; set;}
	public bool Deleted {get; set;}

	public ICollection<Feedback>? Feedbacks { get; set; } // = new List<Assignment>();

}
