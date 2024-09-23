using System;

namespace HMS_API.Entities;

public class Feedback
{
	public int Id {get; set;}
    public required int SubID {get; set;}                   //get from submission chosen
    public Submission? Submission {get; set;}               // call on class
	public required int LectID {get; set;}                  // get form user/login
    public UserAccount? UserAccount {get; set;}             // call on class
	public required string Comment {get; set;}
	public DateOnly ReturnDate {get; set;}
	public required int MarkAchieved {get; set;}
	public DateOnly Created {get; set;}
	public DateOnly Modified {get; set;}
	public int Deleted {get; set;}
}
