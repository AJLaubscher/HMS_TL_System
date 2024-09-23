namespace HMS_API.Dtos.userAccount;

public record class UserSummaryDto // return list of all users
(   
    int Id,                       
	string Username,
	string UserPassword,
	string FName,
	string LName,
	int UserRole,
	DateOnly Created,
	DateOnly Modified,
	int Deleted
);
