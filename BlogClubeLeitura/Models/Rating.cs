using System;
using System.ComponentModel.DataAnnotations;

namespace BlogClubeLeitura.Models
{
    public class Rating
    {
        private DateTime _ratingDate;

        public int Id { get; set; }
        public int? Stars { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public DateTime RatingDate
        {
            get => _ratingDate;
            set => _ratingDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }


        public int PostId { get; set; }
        public Post? Post { get; set; }
    }
}