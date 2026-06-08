namespace football_management_system_cscb.Models.MatchSimulation
{
    public class CornerEvent : MatchEvent
    {
        public CornerEvent() {
            EventType = MatchEventType.Corner;
        }

        public string TeamName { get; set; } = "";

        public string TakerName { get; set; } = "";
    }
}