using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Model.Dto
{
    public class PrivacyDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Lang { get; set; } = "hu";
        public string? Name { get; set; }
        [Required]
        public string? Text { get; set; }


        public DateTime Date { get; set; } = DateTime.Now;
    }
}
