using Cargotruck.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cargotruck.Shared.Models
{
    public class UpdateRequest
    {
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string Role { get; set; } = "User";

        public string Img { get; set; } = "/img/profile.png";
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Error_password", ErrorMessageResourceType = typeof(Resource))]
        [Compare(nameof(Password), ErrorMessageResourceName = "PasswordConfirm_error", ErrorMessageResourceType = typeof(Resource))]
        public string PasswordConfirm { get; set; }
    }
}
