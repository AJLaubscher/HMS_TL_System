namespace HMS_API.Dtos.sumission;

public record class SubmissionDetailsDto
(
	int Id,
	int StudID,
	int AssignID,
	DateOnly SubDate,
	string FilePath,
	int Marked,
	DateOnly Created,
	DateOnly Modified,
	int Deleted
);
