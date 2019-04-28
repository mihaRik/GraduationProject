using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Image
    {
        public int Id { get; set; }

        [StringLength(300)]
        public string ImageUrl { get; set; }

        public int BookId { get; set; }

        public virtual Book Book { get; set; }
    }
}
