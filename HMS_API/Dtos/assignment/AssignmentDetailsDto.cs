namespace HMS_API.Dtos.assignment;

public record class AssignmentDetailsDto
(
	int Id,
	int ModID, 
	string Title,
	string Instructions, 
	DateOnly OpenDate, 
	DateOnly DueDate, 
	int MaxMarks, 
	string SubPath,
	DateOnly Created, 
	DateOnly Modified, 
	bool Deleted
);