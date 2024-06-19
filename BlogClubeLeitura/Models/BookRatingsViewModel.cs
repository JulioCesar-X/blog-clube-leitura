namespace BlogClubeLeitura.Models
{
    public class BookRatingsViewModel
    {
        public string? BookTitle { get; set; }
        public int BookId { get; set; }
        public int FiveStarsCount { get; set; }
        public int FourStarsCount { get; set; }
        public int ThreeStarsCount { get; set; }
        public int TwoStarsCount { get; set; }
        public int OneStarCount { get; set; }
        public string? CoverImagePath { get; set; }
    }
}
