using Cargotruck.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cargotruck.Shared.Models
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessageResourceName = "Error_password", ErrorMessageResourceType = typeof(Resource))]
        public string? Password { get; set; }
        [Required(ErrorMessageResourceName = "Error_password", ErrorMessageResourceType = typeof(Resource))]
        public string? PasswordCurrent { get; set; }
    }
}
