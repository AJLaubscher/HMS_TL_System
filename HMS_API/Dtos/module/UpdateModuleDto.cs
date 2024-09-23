namespace HMS_API.Dtos.module;

public record class UpdateModuleDto
(
	//Id
	string Code,
	string ModName,
	int LectID,
	//DateOnly Created,         non changing value
	DateOnly Modified,
	int Deleted
);
