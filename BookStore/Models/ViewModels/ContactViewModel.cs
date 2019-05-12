using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModels
{
    public class ContactViewModel
    {
        public Banner Banner { get; set; }

        public Contact Contact { get; set; }

        public ApplicationUser User { get; set; }

        public bool IsSent { get; set; }
    }
}
