using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<ApplicationUser> Users { get; set; }

        public IEnumerable<Book> LastFiveAddedBooks { get; set; }

        public IEnumerable<Comment> LastFiveComments { get; set; }

        public IEnumerable<Book> LastFiveRatedBooks { get; set; }
    }
}
