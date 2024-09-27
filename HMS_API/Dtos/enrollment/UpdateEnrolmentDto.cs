using System.ComponentModel.DataAnnotations;

namespace HMS_API.Dtos.enrollment;

public record class UpdateEnrolmentDto
(
	[Required] int ModID,
	[Required] int StudID,
	DateOnly Enrol_year,
	// DateOnly Created,
	DateOnly Modified,
	bool Deleted
);
