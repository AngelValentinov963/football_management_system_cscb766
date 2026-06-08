namespace football_management_system_cscb.Models.MatchSimulation
{
    public class ShotEvent : MatchEvent
    {
        public ShotEvent()
        {
            EventType = MatchEventType.Shot;
        }
        public string TeamName { get; set; } = "";

        public string ShooterName { get; set; } = "";

        public bool OnTarget { get; set; }

        public bool Goal { get; set; }
    }
}