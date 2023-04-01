using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Model
{
    public class Login
    {
        [Required]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime LoginDate { get; set; }
    }
}
