using System;

namespace HMS_API.Entities;

public class Assignment
{
    public int Id {get; set;}
	public required int ModID {get; set;}                    
    public Module? Module {get; set;}                   // call class
	public required string Title {get; set;}
	public required string Instructions {get; set;}
	public required DateOnly OpenDate {get; set;}
	public required DateOnly DueDate {get; set;}
	public required int MaxMarks {get; set;}
	public required string SubPath {get; set;}
	public DateOnly Created {get; set;}
	public DateOnly Modified {get; set;}
	public int Deleted {get; set;}

	public ICollection<Submission>? Submissions { get; set; } // = new List<Submission>();
}
