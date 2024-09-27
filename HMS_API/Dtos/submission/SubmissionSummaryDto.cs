namespace HMS_API.Dtos.submission;

public record class SubmissionSummaryDto
(
	int Id,
	int StudID,
    string Name,                // student name
	int AssignID,               
    string Title,               // assignment title
	DateOnly SubDate,
	string FilePath,
	bool Marked,
	DateOnly Created,
	DateOnly Modified,
	bool Deleted
);
