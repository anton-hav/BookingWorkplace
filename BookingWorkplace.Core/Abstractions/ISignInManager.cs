using System.Security.Claims;

namespace BookingWorkplace.Core.Abstractions;

public interface ISignInManager
{
    Task SignOutAsync();
    bool IsSignedIn();
}