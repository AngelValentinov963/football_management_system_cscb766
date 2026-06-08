namespace football_management_system_cscb.Models.MatchSimulation
{
    public class InjuryEvent : MatchEvent
    {
        public InjuryEvent()
        {
            EventType = MatchEventType.Injury;
        }
        public string PlayerName { get; set; } = "";

        public string TeamName { get; set; } = "";

        public string InjuryType { get; set; } = "";

        public bool RequiresSubstitution { get; set; }
    }
}