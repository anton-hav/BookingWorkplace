using AutoMapper;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Core.DataTransferObjects;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
using BookingWorkplace.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookingWorkplace.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ISignInManager _signInManager;
        private readonly IUserManager _userManager;


        public AccountController(IUserService userService, 
            IMapper mapper, 
            ISignInManager signInManager, 
            IUserManager userManager)
        {
            _userService = userService;
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

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
                        await Authenticate(model.Email);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CheckEmailForExistence(string email)
        {
            try
            {
                var isExist = await _userService.IsUserExistsAsync(email);
                if (isExist)
                {
                    return Ok(false);
                }
                return Ok(true);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

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

        [HttpPost]
        public async Task<IActionResult> CheckLoginData(string password, string email)
        {
            try
            {
                var isPasswordCorrect = await _userService.CheckUserPasswordAsync(email, password);
                if (!isPasswordCorrect)
                {
                    return Ok(false);
                }
                return Ok(true);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return StatusCode(500);
            }
        }
        

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            Log.Information("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        private async Task Authenticate(string email)
        {
            var userDto = await _userService.GetUserByEmailAsync(email);

            var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userDto.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, userDto.Role.Name)
            };

            var identity = new ClaimsIdentity(claims,
                "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
            );

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserData()
        {
            var dto = await _userManager.GetUserAsync();

            var user = _mapper.Map<UserDataModel>(dto);
            return Ok(user);
        }
    }
}
