using System.ComponentModel.DataAnnotations;
namespace HMS_API.Dtos.userAccount;

public record class CreateUserDto
(   //comments indicates database default values/ inserted by each creation
    //int Id,                       
	[Required] [StringLength(20)] string Username,
	[Required] [StringLength(10, MinimumLength = 8)] string UserPassword,
	[Required] [StringLength(20)] string FName,
	[Required] [StringLength(20)] string LName,
	[Required] int UserRole
	//DateOnly Created,
	//DateOnly Modified,
	//int Deleted
);
