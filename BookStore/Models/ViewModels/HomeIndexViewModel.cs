using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Book> Slider { get; set; }

        public IEnumerable<Book> MostFavoriteBooks { get; set; }

        public IEnumerable<Book> MostCommentedBooks { get; set; }

        public IEnumerable<Book> MostViewedBooks { get; set; }

        public IEnumerable<Book> MostRatedBooks { get; set; }

        public Banner Banner { get; set; }
    }
}
