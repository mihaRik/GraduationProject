using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class Images
    {
        public int Id { get; set; }

        [StringLength(300)]
        public string ExtraLarge { get; set; }

        [StringLength(300)]
        public string Large { get; set; }

        [StringLength(300)]
        public string Meddium { get; set; }

        [StringLength(300)]
        public string SmallThumbnail { get; set; }

        [StringLength(300)]
        public string Thumbnail { get; set; }

        public int BookId { get; set; }

        public virtual Book Book { get; set; }
    }
}
