using Cargotruck.Server.Data;
using Cargotruck.Server.Models;
using Cargotruck.Server.Services.Interfaces;
using Cargotruck.Shared.Model;
using Cargotruck.Shared.Model.Dto;
using Cargotruck.Shared.Resources;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Globalization;
using System.Security.Claims;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Cargotruck.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult> LoginAsync(LoginRequest request, CultureInfo lang)
        {
            var result = await _authService.LoginAsync(request, lang);

            if (result == null)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RegisterAsync(RegisterRequest parameters)
        {
            var result = await _authService.RegisterAsync(parameters);

            if (result == null)
            {
                return LocalRedirect("/Admin");
            }
            else
            {
                return BadRequest(result);
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> UpdateAsync(UpdateRequest parameters)
        {
            var result = await _authService.UpdateAsync(parameters);

            if (result == null)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordRequest parameters)
        {
            var result = await _authService.ChangePasswordAsync(parameters);

            if (result == null)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost]
        public async Task<ActionResult> LogoutAsync()
        {
            await _authService.LogoutAsync();
            return Ok();
        }

        [HttpGet]
        public CurrentUser CurrentUserInfo()
        {
           return _authService.CurrentUserInfo();
        }
    }

}
