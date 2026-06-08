using football_management_system_cscb.Models.Season;

namespace football_management_system_cscb.Services
{
    public class SeasonService
    {
        public List<Fixture> GenerateSeasonFixtures(List<int> teamIds, int seasonId)
        {
            var fixtures = new List<Fixture>();
            var teams = new List<int>(teamIds);

            if (teams.Count % 2 != 0)
                teams.Add(-1);

            int n = teams.Count;
            int rounds = n - 1;

            var rotation = new List<int>(teams);
            int week = 1;

            // FIRST LEG
            for (int round = 0; round < rounds; round++)
            {
                for (int i = 0; i < n / 2; i++)
                {
                    int home = rotation[i];
                    int away = rotation[n - 1 - i];

                    if (home == -1 || away == -1)
                        continue;

                    fixtures.Add(new Fixture
                    {
                        Week = week,
                        HomeTeamId = home,
                        AwayTeamId = away,
                        SeasonId = seasonId
                    });
                }

                Rotate(rotation);
                week++;
            }

            // SECOND LEG
            var firstLeg = fixtures.ToList();

            foreach (var f in firstLeg)
            {
                fixtures.Add(new Fixture
                {
                    Week = f.Week + rounds,
                    HomeTeamId = f.AwayTeamId,
                    AwayTeamId = f.HomeTeamId,
                    SeasonId = seasonId
                });
            }

            return fixtures;
        }

        private void Rotate(List<int> list)
        {
            int last = list[^1];
            list.RemoveAt(list.Count - 1);
            list.Insert(1, last);
        }
    }
}