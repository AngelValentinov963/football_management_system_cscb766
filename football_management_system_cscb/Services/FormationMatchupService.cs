using football_management_system_cscb.Models.Formation;

namespace football_management_system_cscb.Services
{
    public class FormationMatchupService
    {
        public double GetAdvantage(Formation home, Formation away)
        {
            if (home.Name == "4-3-3" && away.Name == "3-5-2")
                return 0.10;

            if (home.Name == "3-5-2" && away.Name == "4-3-3")
                return -0.10;

            if (home.Name == "4-4-2" && away.Name == "4-3-3")
                return -0.05;

            if (home.Name == "4-3-3" && away.Name == "4-4-2")
                return 0.05;

            if (home.Name == "3-5-2" && away.Name == "4-4-2")
                return 0.08;

            if (home.Name == "4-4-2" && away.Name == "3-5-2")
                return -0.08;

            return 0;
        }
    }
}
