namespace HMS_API.Dtos.feedback;

public record class FeedbackSummaryDto
(
	int Id,
    int SubID,              //get from submission chosen
	int LectID,             // get form user/login
    string LectName,        // get lecturer name from user table
	string Comment,
	DateOnly ReturnDate,
	int MarkAchieved,
	DateOnly Created,
	DateOnly Modified,
	int Deleted
);
