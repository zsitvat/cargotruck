using Cargotruck.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cargotruck.Shared.Models
{
    public class Logins
    {
        [Required]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime LoginDate { get; set; }
    }
}
