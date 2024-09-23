namespace HMS_API.Dtos.enrollment;

public record class UpdateEnrolmentDto
(
	int ModID,
	int StudID,
	DateOnly Enrol_year,
	// DateOnly Created,
	DateOnly Modified,
	int Deleted
);
