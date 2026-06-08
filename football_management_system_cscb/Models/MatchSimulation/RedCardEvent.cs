namespace football_management_system_cscb.Models.MatchSimulation
{
    public class RedCardEvent : MatchEvent
    {
        public RedCardEvent()
        {
            EventType = MatchEventType.RedCard;
        }
        public string PlayerName { get; set; } = "";

        public string TeamName { get; set; } = "";

        public string Reason { get; set; } = "";
    }
}