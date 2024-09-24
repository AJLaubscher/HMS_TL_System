using System.ComponentModel.DataAnnotations;

namespace HMS_API.Dtos.feedback;

public record class UpdateFeedbackDto
(
    [Required] int SubID,             
	[Required] int LectID,             
	[Required] [StringLength(100)] string Comment,
	[Required] DateOnly ReturnDate,
	[Required] int MarkAchieved,
	// DateOnly Created,
	[Required] DateOnly Modified,
	int Deleted
);
