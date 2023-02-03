using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Models
{
    public class Privacies
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Lang { get; set; }
        public string? Name { get; set; }
        [Required]
        public string? Text { get; set; }


        public DateTime Date { get; set; } = DateTime.Now;
    }
}
