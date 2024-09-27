namespace HMS_API.Dtos.assignment;

public record class AssignmentSummaryDto
(
	int Id,
	int ModID,
    string ModName,             // module name
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
