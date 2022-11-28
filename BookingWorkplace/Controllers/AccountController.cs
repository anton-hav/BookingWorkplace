using AutoMapper;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using BookingWorkplace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BookingWorkplace.Controllers;

/// <summary>
///     Controller providing the account operation.
/// </summary>
public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly ISignInManager _signInManager;
    private readonly IUserManager _userManager;
    private readonly IBookingEventHandler _bookingEventHandler;


    public AccountController(IUserService userService,
        IMapper mapper,
        ISignInManager signInManager,
        IUserManager userManager,
        IBookingEventHandler bookingEventHandler)
    {
        _userService = userService;
        _mapper = mapper;
        _signInManager = signInManager;
        _userManager = userManager;
        _bookingEventHandler = bookingEventHandler;
    }

    /// <summary>
    ///     Shows a Register View
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    /// <summary>
    ///     Processes the registration model, if the model is valid creates a new user, if not valid shows a Register View
    /// </summary>
    /// <param name="model">a register model as a <see cref="RegisterModel" /></param>
    /// <returns><see cref="ViewResult" /> for response</returns>
    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            var userDto = _mapper.Map<UserDto>(model);
            if (userDto != null)
            {
                var result = await _userService.RegisterUserAsync(userDto);
                if (result > 0)
                {
                    await _bookingEventHandler.ReportNewUserRegistration(model.Email);
                    await Authenticate(model.Email);
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        return View(model);
    }

    /// <summary>
    ///     Checks email for existence in the data source.
    /// </summary>
    /// <param name="email">user email as a string</param>
    /// <returns><see cref="OkObjectResult" /> if does not exist or <see cref="StatusCodeResult" /> if exists</returns>
    [HttpPost]
    public async Task<IActionResult> CheckEmailForExistence(string email)
    {
        try
        {
            var isExist = await _userService.IsUserExistsAsync(email);
            if (isExist) return Ok(false);
            return Ok(true);
        }
        catch (Exception e)
        {
            Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
            return StatusCode(500);
        }
    }

    /// <summary>
    ///     Shows a Login View
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    /// <summary>
    ///     Processes the login model, if the model is valid authenticate a new user, if not valid shows a Login View
    /// </summary>
    /// <param name="model">a login model as a <see cref="LoginModel" /></param>
    /// <returns><see cref="ViewResult" /> if model does not valid or <see cref="RedirectToActionResult" /></returns>
    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            await Authenticate(model.Email);
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    /// <summary>
    ///     Validate user login data.
    /// </summary>
    /// <param name="password">a user password as a string</param>
    /// <param name="email">a user email as a string</param>
    /// <returns>
    ///     <see cref="OkObjectResult" />
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> CheckLoginData(string password, string email)
    {
        try
        {
            var isPasswordCorrect = await _userService.CheckUserPasswordAsync(email, password);
            if (!isPasswordCorrect) return Ok(false);
            return Ok(true);
        }
        catch (Exception e)
        {
            Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
            return StatusCode(500);
        }
    }

    /// <summary>
    ///     Logout
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        Log.Information("User logged out.");
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    ///     Gets user login preview as a response
    /// </summary>
    /// <returns>
    ///     <see cref="ViewResult" />
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> UserLoginPreview()
    {
        if (_signInManager.IsSignedIn())
        {
            var dto = await _userManager.GetUserAsync();
            var user = _mapper.Map<UserDataModel>(dto);
            return View(user);
        }

        return View();
    }

    /// <summary>
    ///     Gets a user data
    /// </summary>
    /// <returns><see cref="OkObjectResult" /> with <see cref="UserDataModel" /></returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUserData()
    {
        var dto = await _userManager.GetUserAsync();

        var user = _mapper.Map<UserDataModel>(dto);
        return Ok(user);
    }

    /// <summary>
    ///     Authenticate user by email.
    /// </summary>
    /// <param name="email">a user email as a string</param>
    /// <returns>The Task</returns>
    private async Task Authenticate(string email)
    {
        var userDto = await _userService.GetUserByEmailAsync(email);

        await _signInManager.SignInAsync(userDto);
    }
}