
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace football_management_system_cscb.Models
{
    public class Team
    {
        [Key]
        [Column("team_id")]
        public int TeamId { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("city")]
        public string City { get; set; }
        [Column("foundation_year")]
        public string FoundationYear { get; set; }

        [Column("stadium")]
        public string Stadium { get; set; }

        [Column("team_logo")]
        public string LogoUrl { get; set; }

        public List<Player> Players { get; set; } = new();
        public List<FootballMatch> HomeMatches { get; set; } = new();
        public List<FootballMatch> AwayMatches { get; set; } = new();

        [Column("default_formation")]
        public string DefaultFormation { get; set; }


        [Column("Budget")]
        public decimal Budget { get; set; }

    }
}