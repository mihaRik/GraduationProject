using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class RegisterModel
    {
        [StringLength(150), Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(8, ErrorMessage = "Minimal length 8 symbols")]
        public string Password { get; set; }

        [Compare(nameof(Password)), Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}
