using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Models
{
    public class Privacies
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? lang { get; set; }
        public string? name { get; set; }
        [Required]
        public string? text { get; set; }


        public DateTime Date { get; set; } = DateTime.Now;
    }
}
