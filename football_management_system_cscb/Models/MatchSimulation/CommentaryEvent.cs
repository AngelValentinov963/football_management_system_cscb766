using static System.Net.Mime.MediaTypeNames;

namespace football_management_system_cscb.Models.MatchSimulation
{
    public class CommentaryEvent : MatchEvent
    {
        public CommentaryEvent() { 
            EventType = MatchEventType.Commentary;
            
        }
        private CommentaryEvent CreateCommentary(int minute, string text)
        {
            return new CommentaryEvent
            {
                Minute = minute,
                Description = text
            };
        }
    }
}
