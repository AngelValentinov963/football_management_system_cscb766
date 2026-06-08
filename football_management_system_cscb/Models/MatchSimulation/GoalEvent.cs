namespace football_management_system_cscb.Models.MatchSimulation
{
    public class GoalEvent : MatchEvent
    {
        public GoalEvent()
        {
            EventType = MatchEventType.Goal;
        }
        public string TeamName { get; set; } = "";

        public string ScorerName { get; set; } = "";

        public string? AssistName { get; set; }
    }
}