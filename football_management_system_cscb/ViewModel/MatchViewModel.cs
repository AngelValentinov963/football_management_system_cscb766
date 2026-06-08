using football_management_system_cscb.Models;

namespace football_management_system_cscb.ViewModel;

public class MatchViewModel
{
    public string HomeTeamName { get; set; }
    public string AwayTeamName { get; set; }

    public int HomeGoals { get; set; }
    public int AwayGoals { get; set; }

    public int CurrentMinute { get; set; }

    public List<MatchEvent> Events { get; set; } = new();
}