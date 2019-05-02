using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModels
{
    public class BookDetailsViewModel
    {
        public Book Book { get; set; }

        public IEnumerable<Book> RecommendedBooks { get; set; }
    }
}
