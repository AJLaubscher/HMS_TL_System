namespace HMS_API.Dtos.submission;

public record class UpdateSubmissionDto
(
	// int Id
	int StudID,
	int AssignID,
	DateOnly SubDate,
	string FilePath,
	int Marked,
	//DateOnly Created,
	DateOnly Modified,
	int Deleted
);
