using Cargotruck.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Cargotruck.Shared.Model
{
    public class Setting
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? SettingName { get; set; }
        [Required(ErrorMessageResourceName = "Error_setting_value", ErrorMessageResourceType = typeof(Resource))]
        public string? SettingValue { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

    }
}
