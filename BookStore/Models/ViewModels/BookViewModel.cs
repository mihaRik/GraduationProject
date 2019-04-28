using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModels
{
    public class BookViewModel
    {
        public Book Book { get; set; }

        public IEnumerable<int> CategoriesId { get; set; }

        public IEnumerable<int> AuthorsId { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public IEnumerable<Author> Authors { get; set; }
    }
}
