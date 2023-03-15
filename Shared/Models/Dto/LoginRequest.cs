using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Models.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessageResourceName = "Error_username", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Username", ResourceType = typeof(Resource))]
        public string? UserName { get; set; }
        [Required(ErrorMessageResourceName = "Error_password", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Password", ResourceType = typeof(Resource))]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
