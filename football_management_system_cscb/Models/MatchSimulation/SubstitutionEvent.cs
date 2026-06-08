using football_management_system_cscb.Models.MatchSimulation;

namespace football_management_system_cscb.Models.MatchSimulation.Events
{
    public class SubstitutionEvent : MatchEvent
    {
        public SubstitutionEvent()
        {
            EventType = MatchEventType.Substitution ;
        }

        public string TeamName { get; set; } = "";

        public string PlayerOff { get; set; } = "";

        public string PlayerOn { get; set; } = "";
    }
}