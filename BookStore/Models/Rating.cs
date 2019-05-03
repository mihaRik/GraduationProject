using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int RatingValue { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string UserId { get; set; }

        public virtual Book Book { get; set; }

        public int BookId { get; set; }
    }
}
