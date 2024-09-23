namespace HMS_API.Dtos.userAccount;

public record class UpdateUserDto
(
    //int Id,                   // will be searched to find user
	string Username,
	string UserPassword,
	string FName,
	string LName,
	int UserRole,
	//DateOnly Created,         // omit from any updates
	DateOnly Modified,          // last changed/updated
	int Deleted                 // soft delete
);
