using System;

namespace HMS_API.Entities;

public class Module
{
	public int Id {get; set;}
	public required string Code {get; set;}
	public required string ModName {get; set;}
	public required int LectID {get; set;}
    public UserAccount? UserAccount {get; set;}                 // call user class
	public DateOnly Created {get; set;}
	public DateOnly Modified {get; set;}
	public int Deleted {get; set;}

	public ICollection<Enrolment>? Enrolments { get; set; } // = new List<Enrolment>();
	public ICollection<Assignment>? Assignments { get; set; } // = new List<Assignment>();

}
