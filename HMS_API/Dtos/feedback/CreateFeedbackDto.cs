namespace HMS_API.Dtos.feedback;

public record class CreateFeedbackDto 
(
    int SubID,              //get from submission chosen
	int LectID,             // get form user/login
	string Comment,
	//DateOnly ReturnDate,
	int MarkAchieved
	// DateOnly Created,
	// DateOnly Modified,
	// int Deleted
);
