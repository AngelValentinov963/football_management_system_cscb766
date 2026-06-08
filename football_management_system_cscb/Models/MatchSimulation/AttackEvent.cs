namespace football_management_system_cscb.Models.MatchSimulation
{
    public class AttackEvent : MatchEvent
    {

        public AttackEvent() {
            this.EventType = MatchEventType.Attack;
        }

        public string TeamName { get; set; } = "";

        public string CreatorName { get; set; } = "";

        public string FinisherName { get; set; } = "";
    }
}