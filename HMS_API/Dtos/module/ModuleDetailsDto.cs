namespace HMS_API.Dtos.module;

public record class ModuleDetailsDto
(
	int Id,
	string Code,
	string ModName,
	int LectID,
	DateOnly Created,
	DateOnly Modified,
	bool Deleted
);
