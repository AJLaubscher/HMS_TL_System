namespace HMS_API.Dtos.module;

public record class ModuleSummaryDto
(
	int Id,
	string Code,
	string ModName,
	int LectID,
    string LectName,         // pass lecturer name as well
	DateOnly Created,
	DateOnly Modified,
	int Deleted
);
