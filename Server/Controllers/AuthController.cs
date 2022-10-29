﻿using Cargotruck.Client;
using Cargotruck.Server.Models;
using Cargotruck.Shared.Models;
using Cargotruck.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
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


        public AuthController(IStringLocalizer<Resource> localizer, UserManager<Users> userManager, SignInManager<Users> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!_userManager.Users.Any())
            {
                var admin = new Users();
                admin.UserName = "admin";
                var result = await _userManager.CreateAsync(admin, "Admin123*");
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
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("img", parameters.Img));
            if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
            return LocalRedirect("/Admin");
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

            return new CurrentUser
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name,
                Claims = User.Claims
                    .ToDictionary(c => c.Type, c => c.Value),
            };
        }
    }

}
