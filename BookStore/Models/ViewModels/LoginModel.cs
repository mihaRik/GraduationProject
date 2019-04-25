using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class LoginModel
    {
        [Required, StringLength(150), EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(8, ErrorMessage = "Minimal length 8 symbols")]
        public string Password { get; set; }

        [Display(Name = "Remmember me?")]
        public bool RemmemberMe { get; set; }
    }
}
