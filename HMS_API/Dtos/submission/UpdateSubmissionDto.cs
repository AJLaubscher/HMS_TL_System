using System.ComponentModel.DataAnnotations;

namespace HMS_API.Dtos.submission;

public record class UpdateSubmissionDto
(
	// int Id
	[Required] int StudID,
	[Required] int AssignID,
	[Required] DateOnly SubDate,
	string FilePath,
	bool Marked,
	//DateOnly Created,
	[Required] DateOnly Modified,
	bool Deleted
);
