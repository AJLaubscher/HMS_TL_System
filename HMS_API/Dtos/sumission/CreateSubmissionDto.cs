namespace HMS_API.Dtos.sumission;

public record class CreateSubmissionDto
(
	// int Id
	int StudID,                         // get from user/login
	int AssignID,                       // get from chosen assignment
	//DateOnly SubDate,
	string FilePath                     // construct filepath to exact submission
	//int Marked,
	//DateOnly Created,
	//DateOnly Modified,
	//int Deleted
);
