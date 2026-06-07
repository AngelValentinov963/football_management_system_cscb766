namespace football_management_system_cscb.ViewModels;

public class PlayerViewModel
{
    public int PlayerId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public string? Nationality { get; set; }

    public int? OverallRating { get; set; }

    public int? Potential { get; set; }

    public string? PreferredPosition { get; set; }

    public decimal? MarketValue { get; set; }

    public bool IsInSquad { get; set; }
}