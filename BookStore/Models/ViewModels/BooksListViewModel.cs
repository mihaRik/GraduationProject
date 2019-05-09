using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModels
{
    public class BooksListViewModel
    {
        public IEnumerable<Book> Books { get; set; }

        public int Total { get; set; }

        public int PageCount { get; set; }

        public int CurrentPage { get; set; }

        public string Search { get; set; }

        public int CategoryId { get; set; }

        public int AuthorId { get; set; }

        public int Rate { get; set; }
    }
}
