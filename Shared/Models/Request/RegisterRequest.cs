using Cargotruck.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cargotruck.Shared.Models.Request
{
    public class RegisterRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceName = "Error_password", ErrorMessageResourceType = typeof(Resource))]
        [StringLength(30, MinimumLength = 8, ErrorMessageResourceName = "Error_password_lenght", ErrorMessageResourceType = typeof(Resource))]
        [DataType(DataType.Password, ErrorMessageResourceName = "Error_password_validation", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; }
        public string Role { get; set; } = "User";

        public string Img { get; set; } = "/img/profile.jpg";
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Error_password", ErrorMessageResourceType = typeof(Resource))]
        [Compare(nameof(Password), ErrorMessageResourceName = "PasswordConfirm_error", ErrorMessageResourceType = typeof(Resource))]
        public string PasswordConfirm { get; set; }
    }
}
