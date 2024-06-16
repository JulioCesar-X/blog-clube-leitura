using System;

namespace BlogClubeLeitura.Models
{
    public class SeedHistory
    {
        private DateTime _appliedOn;

        public int Id { get; set; }
        public string SeedVersion { get; set; } = string.Empty;
        public DateTime AppliedOn
        {
            get => _appliedOn;
            set => _appliedOn = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}