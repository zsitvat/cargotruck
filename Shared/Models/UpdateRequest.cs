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
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; } = "User";

        public string? Img { get; set; } = "/img/profile.png";
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

    }
}
