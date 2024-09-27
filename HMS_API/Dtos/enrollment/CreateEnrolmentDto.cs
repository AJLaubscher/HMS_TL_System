using System.ComponentModel.DataAnnotations;

namespace HMS_API.Dtos.enrollment;

public record class CreateEnrolmentDto
(
	[Required] int ModID,
	[Required] int StudID
	// DateOnly enrol_year,
	// DateOnly Created,
	// DateOnly Modified,
	// int Deleted
);
