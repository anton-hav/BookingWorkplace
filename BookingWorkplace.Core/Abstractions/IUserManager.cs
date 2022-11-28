using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Core.Abstractions;

public interface IUserManager
{
    Task<UserDto> GetUserAsync();
    Task<Guid> GetUserIdAsync();
    string GetRoleName();
    bool IsUser();
    bool IsAdmin();
}