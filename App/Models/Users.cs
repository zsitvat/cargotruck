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
        [Display(Name = "Felhasználónév")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email megadása kötelező.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Jelszó megadása kötelező.")]
        [Display(Name = "Jelszó")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }   
        [Display(Name = "Jogosultság")]    
        public string Role { get; set; }
        public string LoginErrorMessage { get; set; } = "";
        
    }
}
