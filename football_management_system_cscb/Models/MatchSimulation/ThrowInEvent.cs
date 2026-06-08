namespace football_management_system_cscb.Models.MatchSimulation
{
    public class ThrowInEvent : MatchEvent
    {
        public ThrowInEvent()
        {
            EventType = MatchEventType.ThrowIn;
        }
        public string TeamName { get; set; } = "";

        public string PlayerName { get; set; } = "";
    }
}