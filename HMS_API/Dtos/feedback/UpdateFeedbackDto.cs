namespace HMS_API.Dtos.feedback;

public record class UpdateFeedbackDto
(
    int SubID,             
	int LectID,             
	string Comment,
	DateOnly ReturnDate,
	int MarkAchieved,
	// DateOnly Created,
	DateOnly Modified,
	int Deleted
);
