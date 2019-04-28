using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static BookStore.Data.Roles;

namespace BookStore.Data
{
    public class BookContext : IdentityDbContext<ApplicationUser>
    {
        public BookContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookCategoryPivot> BookCategories { get; set; }
        public DbSet<BookAuthorPivot> BookAuthors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "d1ad6e3c-780f-444f-86e4-1639a100b8a6",
                    Name = Admin,
                    NormalizedName = Admin.ToUpper(),
                    ConcurrencyStamp = "503bfd43-7c5e-422f-9b95-c363a6b7873c"
                },
                new IdentityRole
                {
                    Id = "db467290-239b-4e57-b84a-c675f525dc74",
                    Name = Member,
                    NormalizedName = Member.ToUpper(),
                    ConcurrencyStamp = "5fa3c3e4-79d5-46fb-89ef-41d2a5223119"
                });
        }
    }
}
