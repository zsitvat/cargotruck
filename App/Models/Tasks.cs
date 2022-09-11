using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class Tasks
    {
        [Required]
        public long Id { get; set; }
        [Required]
        public long User_id { get; set; }
        [Required(ErrorMessage = "Partner megadása kötelező.")]
        [Display(Name = "Partner")]
        public string Partner { get; set; }
        [Display(Name = "Leírás")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Az átvétel helyének megadása kötelező.")]
        [Display(Name = "Átvétel helye")]
        public string Place_of_receipt { get; set; }
        [Required(ErrorMessage = "Az átvétel idejének megadása kötelező.")]
        [Display(Name = "Átvétel ideje")]
        public DateTime? Time_of_receipt { get; set; }
        [Required(ErrorMessage = "A leadás helyének megadása kötelező.")]
        [Display(Name = "Leadás helye")]
        public string Place_of_delivery { get; set; }
        [Required(ErrorMessage = "A leadás idejének megadása kötelező.")]
        [Display(Name = "Leadás ideje")]
        public DateTime? Time_of_delivery { get; set; }
        [Display(Name = "Egyéb megállóhelyek")]
        public string Other_stops { get; set; }
        [Display(Name = "Rakomány ID")]
        public string Id_cargo { get; set; }
        [Display(Name = "Raktározás ideje")]
        public string Storage_time { get; set; }
        [Display(Name = "Teljesítve")]
        public bool Completed { get; set; }
        [Display(Name = "Teljesítés ideje")]
        public DateTime? Completion_time { get; set; }
        [Display(Name = "Késés")]
        public string Time_of_delay { get; set; }
        [Display(Name = "Igért összeg")]
        public string Payment { get; set; }
        [Display(Name = "Végleges összeg")]
        public string Final_Payment { get; set; }
        [Display(Name = "Büntetés összege")]
        public string Penalty { get; set; }
        [Display(Name = "Dátum")]
        public DateTime Date { get; set; } = DateTime.Now;

    }   
        
}
