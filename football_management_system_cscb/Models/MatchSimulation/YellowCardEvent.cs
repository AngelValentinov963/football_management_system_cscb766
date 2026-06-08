namespace football_management_system_cscb.Models.MatchSimulation
{
    public class YellowCardEvent : MatchEvent
    {
        public YellowCardEvent()
        {
            EventType = MatchEventType.YellowCard;
        }
        public string PlayerName { get; set; } = "";

        public string TeamName { get; set; } = "";

        public string Reason { get; set; } = "";
    }
}