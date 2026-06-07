using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace football_management_system_cscb.Models
{
    public class FootballMatch
    {  
        [Key]
        [Column("match_id")]
        public int MatchId { get; set; }
        [Column("home_team_id")]
        //foreigh key
        public int HomeTeamId { get; set; }
        [Column("away_team_id")]

        //foreignKey
        public int AwayTeamId { get; set; }
        [Column("home_goals")]
        public int HomeScore { get; set; }
        [Column("away_goals")]
        public int AwayScore { get; set; }
        [Column("match_date")]
        public System.DateTime MatchDate { get; set; }
    }
}
