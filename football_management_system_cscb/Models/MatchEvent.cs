using football_management_system_cscb.Models.MatchSimulation;

namespace football_management_system_cscb.Models
{
    public class MatchEvent
    {
        public int Minute { get; set; }

        public string Description { get; set; } = "";

        public MatchEventType EventType { get; protected set ; }
    }
}
