﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Books.v1.Data;
using Microsoft.AspNetCore.Http;

namespace BookStore.Models
{
    public class Book
    {
        public Book()
        {
            Categories = new HashSet<BookCategoryPivot>();
            Authors = new HashSet<BookAuthorPivot>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string GID { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(200)]
        public string Subtitle { get; set; }

        public string Description { get; set; }

        public int PageCount { get; set; }

        [Required, StringLength(10)]
        public string Language { get; set; }

        public virtual Images Images { get; set; }

        public string DefaultImage
        {
            get
            {
                if (Images != null)
                {
                    return Images.ExtraLarge ??
                           Images.Large ??
                           Images.Meddium ??
                           Images.SmallThumbnail ??
                           Images.Thumbnail;
                }
                else
                {
                    return "img/no-image.png";
                }
            }
        }

        [StringLength(150)]
        public string Publisher { get; set; }

        public DateTime PublishedDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        public virtual ICollection<BookCategoryPivot> Categories { get; set; }

        public virtual ICollection<BookAuthorPivot> Authors { get; set; }

        [StringLength(300)]
        public string Image { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
