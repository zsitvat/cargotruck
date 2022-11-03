using Cargotruck.Client;
using Cargotruck.Data;
using Cargotruck.Server.Models;
using Cargotruck.Shared.Models;
using Cargotruck.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using System.Data.Entity;
using System.Globalization;
using System.Security.Claims;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context,IStringLocalizer<Resource> localizer, UserManager<Users> userManager, SignInManager<Users> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            //if no user, create admin
            if (!_userManager.Users.Any())
            {
                var admin = new Users();
                admin.UserName = "admin";
                admin.Email = "admin@cargotruck.com";
                admin.PhoneNumber = "+421123456789";
                var result = await _userManager.CreateAsync(admin, "Admin123*");
                await _userManager.AddToRoleAsync(admin, "Admin");
                await _userManager.AddClaimAsync(admin, new System.Security.Claims.Claim("img", "/img/profile.png"));
            }

            var password_error = _localizer["Password_error"].Value;
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) return BadRequest("Not found");
            var singInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!singInResult.Succeeded) return BadRequest(password_error);
            await _signInManager.SignInAsync(user, request.RememberMe);
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterRequest parameters)
        {
            var user = new Users();
            user.UserName = parameters.UserName;
            var result = await _userManager.CreateAsync(user, parameters.Password);
            await _userManager.AddToRoleAsync(user, parameters.Role);
            await _userManager.AddClaimAsync(user, new Claim("img", parameters.Img));
            if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
            return LocalRedirect("/Admin");
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateRequest parameters)
        {
            var Claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
            var user = _context.Users.FirstOrDefault(a => a.UserName == User.Identity.Name);
           
            if (user == null) return BadRequest();
            user.UserName = parameters.UserName !=null ? parameters.UserName : user.UserName;
            user.PhoneNumber = parameters.PhoneNumber != null ? parameters.PhoneNumber : user.PhoneNumber; 
            user.Email = parameters.Email != null ? parameters.Email : user.Email;
            var role = Claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
            System.Diagnostics.Debug.WriteLine(role);
            var result = await _userManager.UpdateAsync(user);

            await _userManager.RemoveFromRoleAsync(user, role);
            await _userManager.AddToRoleAsync(user, parameters.Role);
            if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest parameters)
        {
            var user = _context.Users.FirstOrDefault(a => a.UserName == User.Identity.Name);
            var result = await _userManager.ChangePasswordAsync(user, parameters.PasswordCurrent, parameters.Password);
            if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
            return Ok();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet]
        public CurrentUser CurrentUserInfo()
        {
            var u = _context.Users.FirstOrDefault(a => a.UserName == User.Identity.Name);
            if (User.Identity.IsAuthenticated && u != null) { 
                return new CurrentUser
                {
                    IsAuthenticated = User.Identity.IsAuthenticated,
                    UserName = User.Identity.Name,
                    Name = User.Identity.Name,
                    Email =  u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Claims = User.Claims
                        .ToDictionary(c => c.Type, c => c.Value),
                };
            }
            else
            {
                return new CurrentUser { };
            }
        }
    }

}
