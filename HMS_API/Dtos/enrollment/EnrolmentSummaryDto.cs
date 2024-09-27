namespace HMS_API.Dtos.enrollment;

public record class EnrolmentSummaryDto
(
	int ModID,
	string ModName,
	int StudID,
	string FName,
	DateOnly Enrol_year,
	DateOnly Created,
	DateOnly Modified,
	bool Deleted
);
