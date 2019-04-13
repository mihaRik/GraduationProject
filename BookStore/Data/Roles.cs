using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Data
{
    public static class Roles
    {
        private const string admin = "Admin";

        public static string Admin
        {
            get { return admin; }
        }

        private const string member = "Member";

        public static string Member
        {
            get { return member; }
        }
    }
}
