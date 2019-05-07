using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Comments = new HashSet<Comment>();
            Ratings = new HashSet<Rating>();
            Favorites = new HashSet<BookFavoritePivot>();
        }

        [StringLength(100), MinLength(3)]
        public string Firstname { get; set; }

        [StringLength(100), MinLength(3)]
        public string Lastname { get; set; }

        public string Fullname
        {
            get
            {
                return $"{Firstname} {Lastname}";
            }
        }

        public DateTime Birthdate { get; set; }

        [StringLength(300)]
        public string Image { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual IEnumerable<Rating> Ratings { get; set; }

        public virtual ICollection<BookFavoritePivot> Favorites { get; set; }
    }
}
