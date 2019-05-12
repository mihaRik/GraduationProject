using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [StringLength(150), MinLength(3, ErrorMessage = "Minimum legth must be 3 symbols.")]
        public string Firstname { get; set; }

        [StringLength(150), MinLength(3, ErrorMessage = "Minimum legth must be 3 symbols.")]
        public string Lastname { get; set; }

        [StringLength(150), EmailAddress, Required]
        public string Email { get; set; }

        [Required, MinLength(10, ErrorMessage = "Minimum legth must be 10 symbols.")]
        public string Message { get; set; }
    }
}
