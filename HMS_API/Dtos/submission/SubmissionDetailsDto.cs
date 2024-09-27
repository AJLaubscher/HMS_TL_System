namespace HMS_API.Dtos.submission;

public record class SubmissionDetailsDto
(
	int Id,
	int StudID,
	int AssignID,
	DateOnly SubDate,
	string FilePath,
	bool Marked,
	DateOnly Created,
	DateOnly Modified,
	bool Deleted
);
