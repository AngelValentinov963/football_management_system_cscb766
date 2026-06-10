namespace football_management_system_cscb.ViewModels
{
    public class PlayerViewModel
    {
        public int PlayerId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PreferredPosition { get; set; } = string.Empty;

        public string? Nationality { get; set; }

        public DateTime BirthDate { get; set; }

        public int? OverallRating { get; set; }

        public int? Potential { get; set; }

        public decimal? MarketValue { get; set; }


        public bool IsListedForTransfer { get; set; }
    }
}