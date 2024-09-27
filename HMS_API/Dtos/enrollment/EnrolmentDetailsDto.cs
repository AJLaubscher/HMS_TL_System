namespace HMS_API.Dtos.enrollment;

public record class EnrolmentDetailsDto
(
	int ModID,
	int StudID,
	DateOnly Enrol_year,
	DateOnly Created,
	DateOnly Modified,
	bool Deleted
);
