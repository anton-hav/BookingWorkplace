using System.Security.Claims;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BookingWorkplace.IdentityManagers;

public class SignInManager : ISignInManager
{
    private readonly IHttpContextAccessor _contextAccessor;
    private HttpContext _context;

    public SignInManager(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    private HttpContext Context
    {
        get
        {
            var context = _context ?? _contextAccessor?.HttpContext;
            if (context == null) throw new InvalidOperationException("HttpContext must not be null.");
            return context;
        }
        set => _context = value;
    }

    /// <summary>
    ///     Sign out a principal for a default authentication scheme
    /// </summary>
    /// <returns>The Task</returns>
    public async Task SignOutAsync()
    {
        await Context.SignOutAsync();
    }

    /// <summary>
    ///     Sign in a principal for a default authentication scheme
    /// </summary>
    /// <param name="user">
    ///     <see cref="UserDto" />
    /// </param>
    /// <returns>The Task</returns>
    public async Task SignInAsync(UserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, user.Email),
            new(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
        };

        var identity = new ClaimsIdentity(claims,
            "ApplicationCookie",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType
        );

        await Context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }

    /// <summary>
    ///     Checks if the user is signed in
    /// </summary>
    /// <returns>The boolean</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool IsSignedIn()
    {
        var principal = Context.User;
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        return principal.Identities != null && principal.Identities
            .Any(i => i.IsAuthenticated.Equals(true));
    }
}