using System.ComponentModel.DataAnnotations;

namespace HMS_API.Dtos.assignment;

public record class UpdateAssignmentDto
(
	// int Id,
	[Required] int ModID, 
	[Required] [StringLength(30)] string Title,
	[Required] [StringLength(200)] string Instructions, 
	[Required] DateOnly OpenDate, 
	[Required] DateOnly DueDate, 
	[Required] int MaxMarks, 
	string SubPath,
	// DateOnly Created, 
	DateOnly Modified, 
	int Deleted
);
