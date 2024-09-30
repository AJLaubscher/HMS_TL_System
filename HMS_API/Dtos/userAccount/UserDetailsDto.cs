namespace HMS_API.Dtos.userAccount;

public record class UserDetailsDto // passed when posting and updating
(   
    int Id,                       
	string Username,
	string UserPassword,
	string FName,
	string LName,
	int UserRole,
	DateOnly Created,
	DateOnly Modified,
	bool Deleted
);
