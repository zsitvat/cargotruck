using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text.RegularExpressions;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace App.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {

        }
        public IActionResult Language(string culture, string returnUrl)
        {
            Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
               CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
               new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1) }
           );

            return LocalRedirect(returnUrl);
        }
    }
}