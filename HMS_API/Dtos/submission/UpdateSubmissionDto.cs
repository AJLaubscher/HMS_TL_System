using System.ComponentModel.DataAnnotations;

namespace HMS_API.Dtos.submission;

public record class UpdateSubmissionDto
(
	// int Id
	[Required] int StudID,
	[Required] int AssignID,
	[Required] DateOnly SubDate,
	string FilePath,
	int Marked,
	//DateOnly Created,
	[Required] DateOnly Modified,
	int Deleted
);
