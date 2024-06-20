using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogClubeLeitura.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? CoverImagePath { get; set; }
        public int PublishedYear { get; set; }  // Alterado de DateTime para int
        [NotMapped]
        public int? ModeRating { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}