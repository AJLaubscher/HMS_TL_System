using System.ComponentModel.DataAnnotations;

namespace HMS_API.Dtos.submission;

public record class CreateSubmissionDto
(
	// int Id
	[Required] int StudID,                         // get from user/login
	[Required] int AssignID,                       // get from chosen assignment
	//DateOnly SubDate,
	string FilePath                     // construct filepath to exact submission
	//int Marked,
	//DateOnly Created,
	//DateOnly Modified,
	//int Deleted
);
