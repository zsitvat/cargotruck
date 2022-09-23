using App.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models
{
    public class Users
    {
        [Required]
        public int  Id { get; set; } 
        [Required(ErrorMessage = "Felhasználónév megadása kötelező.")]
        [MaxLength(30)]
        [Display(Name = "Username", ResourceType = typeof(Resource))]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Név megadása kötelező.")]
        [MaxLength(30)]
        [Display(Name = "Teljes név")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email megadása kötelező.")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Megfelelő emailt adj meg!")]
        public string Email { get; set; }
        [Display(Name = "Telefonszám")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Jelszó megadása kötelező.")]
        [Display(Name = "Jelszó")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }   
        [Display(Name = "Jogosultság")]    
        public string Role { get; set; }
        public string LoginErrorMessage { get; set; } = "";   
    }
}
