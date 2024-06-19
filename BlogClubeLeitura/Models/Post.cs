using System;
using System.ComponentModel.DataAnnotations;

namespace BlogClubeLeitura.Models
{
    public class Post
    {
        private DateTime _postedDate;

        public int Id { get; set; }
        public string? Comment { get; set; }
        public DateTime PostedDate
        {
            get => _postedDate;
            set => _postedDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
        public int BookId { get; set; }
        public Book? Book { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public Rating? Rating { get; set; } = new Rating();
    }
}