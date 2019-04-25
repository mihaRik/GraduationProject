
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Author
    {
        public Author()
        {
            Books = new HashSet<BookAuthorPivot>();
        }

        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Fullname { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        public virtual ICollection<BookAuthorPivot> Books { get; set; }
    }
}
