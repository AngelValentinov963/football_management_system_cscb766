namespace football_management_system_cscb.Models
{
    public class MatchState
    {
        public int CurrentMinute { get; set; }

        public int HomeGoals { get; set; }

        public int AwayGoals { get; set; }

        public bool IsPaused { get; set; }

        public bool IsFinished { get; set; }

        public List<MatchEvent> Events { get; set; } = new();

        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public string HomeTeamLogoUrl { get; set; }
        public string AwayTeamLogoUrl { get; set; }


        public List<int> HomeActivePlayerIds { get; set; } = new();
        public List<int> AwayActivePlayerIds { get; set; } = new();
        public int HomeShots { get; set; }
        public int AwayShots { get; set; }

        public int HomeShotsOnTarget { get; set; }
        public int AwayShotsOnTarget { get; set; }

        public int HomeCorners { get; set; }
        public int AwayCorners { get; set; }

        public int HomeYellowCards { get; set; }
        public int AwayYellowCards { get; set; }

        public int HomePossession { get; set; }
        public int AwayPossession { get; set; }

        public double Momentum { get; set; }
        public int LastCommentaryMinute { get; set; } = -10;

    }
}
