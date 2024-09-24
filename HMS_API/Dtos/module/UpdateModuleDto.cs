using System.ComponentModel.DataAnnotations;

namespace HMS_API.Dtos.module;

public record class UpdateModuleDto
(
	//Id
	[Required] [StringLength(7)] string Code,
	[Required] [StringLength(20)] string ModName,
	[Required] int LectID,
	//DateOnly Created,         non changing value
	[Required] DateOnly Modified,
	int Deleted
);
