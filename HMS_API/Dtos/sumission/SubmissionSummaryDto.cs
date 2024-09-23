namespace HMS_API.Dtos.sumission;

public record class SubmissionSummaryDto
(
	int Id,
	int StudID,
    string Name,                // student name
	int AssignID,               
    string Title,               // assignment title
	DateOnly SubDate,
	string FilePath,
	int Marked,
	DateOnly Created,
	DateOnly Modified,
	int Deleted
);
