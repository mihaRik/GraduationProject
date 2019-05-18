﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models
{
    public class About
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string Image { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
