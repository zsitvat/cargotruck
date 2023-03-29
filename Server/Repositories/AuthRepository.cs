using Cargotruck.Server.Data;
using Cargotruck.Server.Models;
using Cargotruck.Server.Repositories.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Cargotruck.Shared.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Security.Claims;

namespace Cargotruck.Server.Repositories
{
    public class AuthRepository : IAuthRepository
    {

        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly IStringLocalizer<Resource> _localizer;
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context, IStringLocalizer<Resource> localizer, UserManager<Users> userManager, SignInManager<Users> signInManager)
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
                var admin = new Users
                {
                    UserName = "admin",
                    Email = "admin@cargotruck.com",
                    PhoneNumber = "+421123456789"
                };
                await _userManager.CreateAsync(admin, "Admin123*");
                await _userManager.AddToRoleAsync(admin, "Admin");
                await _userManager.AddClaimAsync(admin, new Claim("img", "img/profile.jpg"));
            }
            CultureInfo.CurrentUICulture = lang;
            var password_error = _localizer["Password_error"].Value;
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null) return _localizer["Not_found"].Value;

            var singInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!singInResult.Succeeded)
                return password_error;

            await _signInManager.SignInAsync(user, request.RememberMe);

            //save login date
            Logins login = new()
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
            Users user = new();
            user.UserName = parameters.UserName;
            user.PhoneNumber = parameters.PhoneNumber ?? user.PhoneNumber;
            user.Email = parameters.Email ?? user.Email;

            var result = await _userManager.CreateAsync(user, parameters.Password);
            await _userManager.AddToRoleAsync(user, parameters.Role);
            await _userManager.AddClaimAsync(user, new Claim("img", parameters.Img));

            if (!result.Succeeded)
                return result.Errors.FirstOrDefault()?.Description;

            return null;
        }
        public async Task<string?> UpdateAsync(UpdateRequest parameters)
        {
            var user = new Users();
            Dictionary<string, string> claims = new();
            Dictionary<string, string> userRoles = new();
            Dictionary<string, string> roles = new();
            string role = "";

            if (parameters.Id != null)
            {
                user = _context.Users.FirstOrDefault(a => a.Id == parameters.Id);
                roles = _context.Roles.ToDictionary(r => r.Id, r => r.Name);
                userRoles = _context.UserRoles.ToDictionary(r => r.UserId, r => roles[r.RoleId]);
                role = userRoles[parameters.Id];
            }
            else
            {
                user = _context.Users.FirstOrDefault(a => a.UserName == _signInManager.Context.User.Identity!.Name);
                claims = _signInManager.Context.User.Claims.ToDictionary(c => c.Type, c => c.Value);
                role = claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
            }

            if (user != null)
            {

                user.UserName = parameters.UserName ?? user.UserName;
                user.PhoneNumber = parameters.PhoneNumber ?? user.PhoneNumber;
                user.Email = parameters.Email ?? user.Email;
                var result = await _userManager.UpdateAsync(user);

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
            Users user = _context.Users.FirstOrDefault(a => a.UserName == _signInManager.Context.User.Identity!.Name)!;
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
                };

            }
            else
            {
                return new CurrentUser { };
            }
        }
    }
}
