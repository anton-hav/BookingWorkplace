using System.Security.Claims;
using BookingWorkplace.Core.DataTransferObjects;

namespace BookingWorkplace.Core.Abstractions;

public interface ISignInManager
{
    Task SignOutAsync();
    Task SignInAsync(UserDto user);
    bool IsSignedIn();
}