using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using Google.Apis.Books.v1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Models
{
    public class Book
    {
        public Book()
        {
            Categories = new HashSet<BookCategoryPivot>();
            Authors = new HashSet<BookAuthorPivot>();
            Images = new HashSet<Image>();
            Comments = new HashSet<Comment>();
            Rating = new HashSet<Rating>();
            Favorites = new HashSet<BookFavoritePivot>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string GID { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(200)]
        public string Subtitle { get; set; }

        public string Description { get; set; }

        [Display(Name = "Page count")]
        public int PageCount { get; set; }

        [Required, StringLength(10)]
        public string Language { get; set; }

        public string DefaultImage
        {
            get
            {
                foreach (var image in Images)
                {
                    if (!string.IsNullOrEmpty(image.ImageUrl)) return image.ImageUrl;
                }

                return "img/no-image.png";
            }
        }

        [StringLength(150)]
        public string Publisher { get; set; }

        [Display(Name = "Published date")]
        public DateTime PublishedDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public virtual ICollection<BookCategoryPivot> Categories { get; set; }

        public virtual ICollection<BookAuthorPivot> Authors { get; set; }

        public virtual ICollection<Rating> Rating { get; set; }

        public virtual ICollection<BookFavoritePivot> Favorites { get; set; }

        public double AvgRating
        {
            get
            {
                if (Rating.Count != 0)
                {
                    return Rating.Average(r => r.RatingValue);
                }

                return 0;
            }
        }

        [NotMapped]
        public IFormFileCollection Photos { get; set; }
    }
}
