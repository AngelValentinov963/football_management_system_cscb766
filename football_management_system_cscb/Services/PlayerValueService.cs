using football_management_system_cscb.Models;

namespace football_management_system_cscb.Services
{
    public class PlayerValueService
    {
        public decimal CalculatePlayerValue(Player p)
        {
            var rating = p.OverallRating ?? 50;
            var potential = p.Potential ?? rating;

            var baseValue = rating * 100_000;

            var potentialBonus = (potential - rating) * 50_000;

            var ageFactor = Math.Max(0.5m, 1 - ((DateTime.Now.Year - p.BirthDate.Year) * 0.02m));

            return Math.Max(50_000, (baseValue + potentialBonus) * ageFactor);
        }
    }
}
