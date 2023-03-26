using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Model.Dto
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessageResourceName = "Error_password", ErrorMessageResourceType = typeof(Resource))]
        public string? Password { get; set; }
        [Required(ErrorMessageResourceName = "Error_password", ErrorMessageResourceType = typeof(Resource))]
        public string? PasswordCurrent { get; set; }
    }
}
