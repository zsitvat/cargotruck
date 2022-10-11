using System.ComponentModel.DataAnnotations;
using Cargotruck.Shared.Resources;

namespace Cargotruck.Shared
{
    public class Users
    {
        [Required]
        public int  Id { get; set; }

        [MaxLength(30)]
        [Display(Name = "Username", ResourceType = typeof(Resource))]
        public string UserName { get; set; } = "";
        [Required(ErrorMessageResourceName = "Error_full_name", ErrorMessageResourceType = typeof(Resource))]
        [MaxLength(30)]
        [Display(Name = "Full_name", ResourceType = typeof(Resource))]
        public string Name { get; set; } = "";
        [Required(ErrorMessageResourceName = "Error_email", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Email", ResourceType = typeof(Resource))]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Megfelelő emailt adj meg!")]
        public string Email { get; set; } = "";
        [Display(Name = "Phone_number", ResourceType = typeof(Resource))]
        public string PhoneNumber { get; set; } = "";
        [Required(ErrorMessageResourceName = "Error_password", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Password", ResourceType = typeof(Resource))]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; } = "";
        [Display(Name = "Role", ResourceType = typeof(Resource))]
        public string Role { get; set; } = "";
    }
}
