﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModels
{
    public class Banner
    {
        public int Id { get; set; }

        [StringLength(300)]
        public string Image { get; set; }

        [StringLength(300), Required]
        public string Title { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
