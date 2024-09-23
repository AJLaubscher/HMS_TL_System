using System;

namespace HMS_API.Entities;

public class Enrolment
{
	public required int ModID {get;set;}
    public Module? Module {get; set;}               // call class
	public required int StudID {get;set;}
    public UserAccount? UserAccount {get; set;}     // call class
	public DateOnly Enrol_year {get;set;}
	public DateOnly Created {get;set;}
	public DateOnly Modified {get;set;}
	public int Deleted {get;set;}
}
