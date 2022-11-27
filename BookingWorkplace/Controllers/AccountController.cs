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
                        await _bookingEventHandler.ReportNewUserRegistration(model.Email);
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
        
        /// <summary>
        /// Authenticate user by email.
        /// </summary>
        /// <param name="email">a user email as a string</param>
        /// <returns>The Task</returns>
        private async Task Authenticate(string email)
        {
            var userDto = await _userService.GetUserByEmailAsync(email);

            await _signInManager.SignInAsync(userDto);
        }
    }
}
