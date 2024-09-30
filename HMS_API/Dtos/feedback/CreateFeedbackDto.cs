using System.ComponentModel.DataAnnotations;

namespace HMS_API.Dtos.feedback;

public record class CreateFeedbackDto 
(
    [Required] int SubID,              //get from submission chosen
	[Required] int LectID,             // get form user/login
	[Required] [StringLength(100)] string Comment,
	//DateOnly ReturnDate,
	[Required] int MarkAchieved
	// DateOnly Created,
	// DateOnly Modified,
	// int Deleted
);
