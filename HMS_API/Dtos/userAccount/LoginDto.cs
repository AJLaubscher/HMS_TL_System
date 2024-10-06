namespace HMS_API.Dtos.userAccount;

public record class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty;
}
