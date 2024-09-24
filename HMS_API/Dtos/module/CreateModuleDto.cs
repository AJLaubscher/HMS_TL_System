using System.ComponentModel.DataAnnotations;
namespace HMS_API.Dtos.module;

public record class CreateModuleDto //comments indicates database default values/ inserted by each creation
(
	//Id
	[Required] [StringLength(7)] string Code,
	[Required] [StringLength(20)] string ModName,
	[Required] int LectID
	//DateOnly Created,
	//DateOnly Modified,
	//int Deleted
);
