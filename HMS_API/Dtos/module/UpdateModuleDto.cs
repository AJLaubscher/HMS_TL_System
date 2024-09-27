using System.ComponentModel.DataAnnotations;

namespace HMS_API.Dtos.module;

public record class UpdateModuleDto
(
	//Id
	[StringLength(7)] string Code,
	[StringLength(20)] string ModName,
	int LectID,
	//DateOnly Created,         non changing value
	 DateOnly Modified,
	bool Deleted
);
