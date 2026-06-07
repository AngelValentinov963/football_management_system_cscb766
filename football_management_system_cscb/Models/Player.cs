using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace football_management_system_cscb.Models
{
    public class Player
    {
        [Key]
        [Column("player_id")]
        public int PlayerId { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Column("birth_date")]
        public DateTime BirthDate { get; set; }

        [Column("nationality")]
        public string? Nationality { get; set; }

        [Column("team_id")]
        public int TeamId { get; set; }

        [Column("overall_rating")]
        public int? OverallRating { get; set; }

        [Column("potential")]
        public int? Potential { get; set; }

        [Column("preferred_position")]
        public string? PreferredPosition { get; set; }

        [Column("market_value")]
        public decimal? MarketValue { get; set; }

        [Column("wage")]
        public decimal? Wage { get; set; }

        [Column("release_clause")]
        public decimal? ReleaseClause { get; set; }

        [Column("body_type")]
        public string? BodyType { get; set; }

        [Column("real_face")]
        public bool? RealFace { get; set; }
    }
}