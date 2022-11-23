using BookingWorkplace.Business.ServiceImplementations;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using System.Security.Authentication;
using System.Security.Claims;

namespace BookingWorkplace.IdentityManagers;

public class UserManager : IUserManager
{
    private readonly IHttpContextAccessor _contextAccessor;
    private HttpContext _context;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public UserManager(IHttpContextAccessor contextAccessor,
        IUserService userService,
        IRoleService roleService)
    {
        _contextAccessor = contextAccessor;
        _userService = userService;
        _roleService = roleService;
    }

    private HttpContext Context
    {
        get
        {
            var context = _context ?? _contextAccessor?.HttpContext;
            if (context == null)
            {
                throw new InvalidOperationException("HttpContext must not be null.");
            }
            return context;
        }
        set => _context = value;
    }

    /// <summary>
    /// Returns the current authorized user. 
    /// </summary>
    /// <returns>The Task&lt;Result&gt; where Result is <see cref="UserDTO"/></returns>
    /// <exception cref="AuthenticationException"></exception>
    public async Task<UserDto> GetUserAsync()
    {
        var email = Context.User.Identity?.Name;
        if (email != null)
        {
            return await _userService.GetUserByEmailAsync(email);
        }

        throw new AuthenticationException(nameof(email));
    }

    /// <summary>
    /// Gets the current authorized user Id.
    /// </summary>
    /// <returns>The Task&lt;Result&gt; where Result is GUID</returns>
    /// <exception cref="AuthenticationException"></exception>
    public async Task<Guid> GetUserIdAsync()
    {
        var email = Context.User.Identity?.Name;
        if (email != null)
        {
            return (await _userService.GetUserByEmailAsync(email)).Id;
        }

        throw new AuthenticationException(nameof(email));
    }

    /// <summary>
    /// Gets the role of the current authorized user.
    /// </summary>
    /// <returns>the role as a string</returns>
    /// <exception cref="AuthenticationException"></exception>
    public string GetRoleName()
    {
        var roleClaim = Context.User.Claims
            .FirstOrDefault(claim => claim.Type
                .Equals(ClaimsIdentity.DefaultRoleClaimType));

        if (roleClaim == null)
        {
            throw new AuthenticationException(nameof(roleClaim));
        }

        return roleClaim.Value;
    }

    /// <summary>
    /// Checks if the current authorized user has a User role.
    /// </summary>
    /// <returns>The Boolean</returns>
    public bool IsUser()
    {
        var roleName = GetRoleName();
        var userRoleNameByDefault = _roleService.GetDefaultRoleNameForUser();
        return roleName.Equals(userRoleNameByDefault);
    }

    /// <summary>
    /// Checks if the current authorized user has a Admin role.
    /// </summary>
    /// <returns>The Boolean</returns>
    public bool IsAdmin()
    {
        var roleName = GetRoleName();
        var adminRoleNameByDefault = _roleService.GetDefaultRoleNameForAdmin();
        return roleName.Equals(adminRoleNameByDefault);
    }
}