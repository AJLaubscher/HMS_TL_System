using System.ComponentModel.DataAnnotations;

namespace HMS_API.Dtos.userAccount;

public record class UpdateUserDto
(
    //int Id,                   // will be searched to find user
	[Required] [StringLength(20)] string Username,
	[Required] [StringLength(20, MinimumLength = 8)] string UserPassword,
	[Required] [StringLength(20)] string FName,
	[Required] [StringLength(20)] string LName,
	[Required] int UserRole,
	//DateOnly Created,         // omit from any updates
	[Required] DateOnly Modified,          // last changed/updated
	bool Deleted                 // soft delete
);
