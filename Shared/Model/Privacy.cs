using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Model
{
    public class Privacies
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Language { get; set; } = "hu";
        public string? Name { get; set; }
        [Required]
        public string? Text { get; set; }


        public DateTime Date { get; set; } = DateTime.Now;
    }
}
