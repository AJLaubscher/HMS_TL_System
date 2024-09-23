using System;
using HMS_API.Dtos.userAccount;
using HMS_API.Entities;

namespace HMS_API.Mapping;

public static class UserAccountMapping
{
    public static UserAccount ToEntity(this CreateUserDto user)
    {
        return new()
        {
            Username = user.Username,
            UserPassword = user.UserPassword,
            FName = user.FName,
            LName = user.LName,
            UserRole = user.UserRole  
        };
    }

    public static UserAccount ToEntity(this UpdateUserDto user, int id)
    {
        return new()
        {
            Id = id,
            Username = user.Username,
            UserPassword = user.UserPassword,
            FName = user.FName,
            LName = user.LName,
            UserRole = user.UserRole  
        };
    }


    public static UserSummaryDto ToUserSummaryDto(this UserAccount user)
    {
        return new
        (
            user.Id,
            user.Username,
            user.UserPassword,
            user.FName,
            user.LName,
            user.UserRole,
            user.Created,
	        user.Modified,
	        user.Deleted
        );
    }

        public static UserDetailsDto ToUserDetailsDto(this UserAccount user)
    {
        return new
        (
            user.Id,
            user.Username,
            user.UserPassword,
            user.FName,
            user.LName,
            user.UserRole,
            user.Created,
	        user.Modified,
	        user.Deleted
        );
    }
}
