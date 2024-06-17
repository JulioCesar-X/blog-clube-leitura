using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogClubeLeitura.Models
{
    public class Book
    {
        private DateTime _publishedDate;

        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? CoverImagePath { get; set; }
        public DateTime PublishedDate
        {
            get => _publishedDate;
            set => _publishedDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
