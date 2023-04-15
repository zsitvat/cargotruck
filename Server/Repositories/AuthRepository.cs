using Cargotruck.Server.Data;
using Cargotruck.Server.Models;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Cargotruck.Shared.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Security.Claims;

namespace Cargotruck.Server.Repositories
{
    public class AuthRepository : IAuthRepository
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context, IStringLocalizer<Resource> localizer, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
            _context = context;
        }

        public async Task<string?> LoginAsync(LoginRequest request, CultureInfo lang)
        {
            //if no user found, create admin
            if (!_userManager.Users.Any())
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@cargotruck.com",
                    PhoneNumber = "+421123456789"
                };
                await _userManager.CreateAsync(admin, "Admin123*");
                await _userManager.AddToRoleAsync(admin, "Admin");
                await _userManager.AddClaimAsync(admin, new Claim("img", "img/profile.jpg"));
            }
            //save client language
            CultureInfo.CurrentUICulture = lang;
            var password_error = _localizer["Password_error"].Value;
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null) return _localizer["Not_found"].Value;

            var singInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!singInResult.Succeeded)
                return password_error;

            await _signInManager.SignInAsync(user, request.RememberMe);

            //save login date
            Login login = new()
            {
                UserName = user.UserName,
                UserId = user.Id,
                LoginDate = DateTime.Now
            };

            _context.Add(login);
            await _context.SaveChangesAsync();

            return null;
        }

        public async Task<string?> RegisterAsync(RegisterRequest parameters)
        {
            //from request create user object
            User user = new();
            user.UserName = parameters.UserName;
            user.PhoneNumber = parameters.PhoneNumber ?? user.PhoneNumber;
            user.Email = parameters.Email ?? user.Email;

            //create user, add roles and claims
            var result = await _userManager.CreateAsync(user, parameters.Password);
            await _userManager.AddToRoleAsync(user, parameters.Role);
            await _userManager.AddClaimAsync(user, new Claim("img", parameters.Img));

            if (!result.Succeeded)
                return result.Errors.FirstOrDefault()?.Description;

            return null;
        }

        public async Task<string?> UpdateAsync(UpdateRequest parameters)
        {
            var user = new User();
            Dictionary<string, string> claims = new();
            Dictionary<string, string> userRoles = new();
            Dictionary<string, string> roles = new();
            string role = "";

            if (parameters.Id != null)
            {
                //if method gets the user id -> search user by id
                user = _context.Users.FirstOrDefault(a => a.Id == parameters.Id);
                roles = _context.Roles.ToDictionary(r => r.Id, r => r.Name);
                userRoles = _context.UserRoles.ToDictionary(r => r.UserId, r => roles[r.RoleId]);
                role = userRoles[parameters.Id];
            }
            else
            {
                //if no id found -> search current user (when admin change his own data)
                user = _context.Users.FirstOrDefault(a => a.UserName == _signInManager.Context.User.Identity!.Name);
                claims = _signInManager.Context.User.Claims.ToDictionary(c => c.Type, c => c.Value);
                role = claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
            }

            if (user != null)
            {

                user.UserName = parameters.UserName ?? user.UserName;
                user.PhoneNumber = parameters.PhoneNumber ?? user.PhoneNumber;
                user.Email = parameters.Email ?? user.Email;
                //update the user
                var result = await _userManager.UpdateAsync(user);

                //change role, but first must be removed
                await _userManager.RemoveFromRoleAsync(user, role);
                await _userManager.AddToRoleAsync(user, parameters.Role);

                if (!result.Succeeded)
                    return result.Errors.FirstOrDefault()?.Description;
                return null;
            }
            else 
            {
                return _localizer["Not_found"].Value;
            } 
        }

        public async Task<string?> ChangePasswordAsync(ChangePasswordRequest parameters)
        {
            User user = _context.Users.FirstOrDefault(a => a.UserName == _signInManager.Context.User.Identity!.Name)!;
            var result = await _userManager.ChangePasswordAsync(user!, parameters.PasswordCurrent, parameters.Password);

            if (!result.Succeeded)
                return result.Errors.FirstOrDefault()?.Description;
            return null;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public CurrentUser CurrentUserInfo()
        {
            //gets the current user info
            var u = _context.Users.FirstOrDefault(a => a.UserName == _signInManager.Context.User.Identity!.Name);

            if (_signInManager.Context.User.Identity!.IsAuthenticated && u != null)
            {
                return new CurrentUser
                {
                    IsAuthenticated = _signInManager.Context.User.Identity!.IsAuthenticated,
                    UserName = _signInManager.Context.User.Identity.Name,
                    Name = _signInManager.Context.User.Identity.Name,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Claims = _signInManager.Context.User.Claims.ToDictionary(c => c.Type, c => c.Value),
                    Id = u.Id,
                    ExpirationDate = DateTime.Now.AddMinutes(0.1)
                };

            }
            else
            {
                return new CurrentUser { };
            }
        }
    }
}
