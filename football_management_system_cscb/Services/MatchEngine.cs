using football_management_system_cscb.Models;
using football_management_system_cscb.Models.Formation;
using football_management_system_cscb.Models.Match;
using football_management_system_cscb.Models.MatchSimulation;
using football_management_system_cscb.Service;
using football_management_system_cscb.Services;

public class MatchEngine
{
    private readonly Random _rng = new();
    private readonly FormationMatchupService _formationMatchup;

    public MatchEngine(FormationMatchupService formationMatchup)
    {
        _formationMatchup = formationMatchup;
    }

   
    public MatchState StartMatch()
    {
        return new MatchState
        {
            CurrentMinute = 0,
            HomeGoals = 0,
            AwayGoals = 0,
            IsPaused = false,
            IsFinished = false,
            Events = new List<MatchEvent>(),
            HomeActivePlayerIds = new List<int>(),
            AwayActivePlayerIds = new List<int>(),
            Momentum = 0
        };
    }

    public void Pause(MatchState state) => state.IsPaused = true;
    public void Resume(MatchState state) => state.IsPaused = false;

    // ==========================
    // MAIN LOOP
    // ==========================
    public void AdvanceMinute(MatchState state, Squad home, Squad away)
    {
        if (state.IsPaused || state.IsFinished)
            return;

        if (!state.HomeActivePlayerIds.Any())
            state.HomeActivePlayerIds = home.StartingXI.Select(p => p.PlayerId).ToList();

        if (!state.AwayActivePlayerIds.Any())
            state.AwayActivePlayerIds = away.StartingXI.Select(p => p.PlayerId).ToList();

        state.CurrentMinute++;

        var homeStrength = CalculateTeamStrength(home);
        var awayStrength = CalculateTeamStrength(away);

        var (homeXg, awayXg) = CalculateXg(home, away, homeStrength, awayStrength);

        double homeChance = homeXg / 90.0;
        double awayChance = awayXg / 90.0;

        state.Momentum += ((homeStrength.Attack - awayStrength.Defense) / 100.0) * 2.0;
        state.Momentum = Math.Clamp(state.Momentum, -100, 100);

        ProcessGoalChance(state, home, away, homeChance, awayChance);
        ProcessRandomEvents(state, home, away);
        ProcessCommentary(state, home, away);

        if (state.CurrentMinute >= 90)
        {
            state.IsFinished = true;

            state.Events.Add(new GenericEvent
            {
                Minute = 90,
                Description =
                    $"🏁 FULL TIME {home.Team.Name} {state.HomeGoals} - {state.AwayGoals} {away.Team.Name}"
            });
        }
    }

    // ==========================
    // GOALS
    // ==========================
    private void ProcessGoalChance(
        MatchState state,
        Squad home,
        Squad away,
        double homeChance,
        double awayChance)
    {
        if (_rng.NextDouble() < homeChance * 0.04)
        {
            state.HomeGoals++;
            var scorer = GetRandomAttacker(home);

            state.Events.Add(new GoalEvent
            {
                Minute = state.CurrentMinute,
                Description = $"⚽ {scorer.FirstName} {scorer.LastName} scores for {home.Team.Name}"
            });
        }

        if (_rng.NextDouble() < awayChance * 0.04)
        {
            state.AwayGoals++;
            var scorer = GetRandomAttacker(away);

            state.Events.Add(new GoalEvent
            {
                Minute = state.CurrentMinute,
                Description = $"⚽ {scorer.FirstName} {scorer.LastName} scores for {away.Team.Name}"
            });
        }
    }

    // ==========================
    // RANDOM EVENTS
    // ==========================
    private void ProcessRandomEvents(MatchState state, Squad home, Squad away)
    {
        double roll = _rng.NextDouble();

        if (roll < 0.03)
        {
            var player = GetRandomPlayer(home, away);

            state.Events.Add(new YellowCardEvent
            {
                Minute = state.CurrentMinute,
                PlayerName = player.FirstName,
                TeamName = home.StartingXI.Any(p => p.PlayerId == player.PlayerId)
                    ? home.Team.Name
                    : away.Team.Name,
                Description = $"🟨 {player.FirstName} {player.LastName} receives a yellow card"
            });
        }
        else if (roll < 0.035)
        {
            var player = GetRandomPlayer(home, away);
            HandleRedCard(state, home, away, player);
        }
    }

    private void HandleRedCard(MatchState state, Squad home, Squad away, Player player)
    {
        bool isHome = home.StartingXI.Any(p => p.PlayerId == player.PlayerId);

        state.Events.Add(new RedCardEvent
        {
            Minute = state.CurrentMinute,
            PlayerName = player.FirstName,
            TeamName = isHome ? home.Team.Name : away.Team.Name,
            Reason = "Professional foul",
            Description = $"🟥 {player.FirstName} {player.LastName} is sent off!"
        });

        if (isHome)
            state.HomeActivePlayerIds.Remove(player.PlayerId);
        else
            state.AwayActivePlayerIds.Remove(player.PlayerId);
    }

    // ==========================
    // COMMENTARY
    // ==========================
    private void ProcessCommentary(MatchState state, Squad home, Squad away)
    {
        if (_rng.NextDouble() > 0.97)
        {
            state.Events.Add(new CommentaryEvent
            {
                Minute = state.CurrentMinute,
                Description = "Midfield battle continues..."
            });
        }
    }

    // ==========================
    // STRENGTH (FIXED SAFE + BALANCED)
    // ==========================
    private TeamMatchStrength CalculateTeamStrength(Squad squad)
    {
        double attack = SafeAvg(squad.StartingXI
            .Where(p => IsAttacker(p.PreferredPosition))
            .Select(p => (p.Shooting + p.Pace + p.Dribbling + (p.OverallRating ?? 50)) / 4.0));

        double midfield = SafeAvg(squad.StartingXI
            .Where(p => IsMidfielder(p.PreferredPosition))
            .Select(p => (p.Passing + p.Dribbling + p.Stamina + (p.OverallRating ?? 50)) / 4.0));

        double defense = SafeAvg(squad.StartingXI
            .Where(p => IsDefender(p.PreferredPosition))
            .Select(p => (p.Defense + p.Stamina + (p.OverallRating ?? 50)) / 3.0));

        var gk = squad.StartingXI.FirstOrDefault(p => p.PreferredPosition == "GK");

        double goalkeeper =
            gk == null
                ? 50
                : (gk.Defense * 0.7 + (gk.OverallRating ?? 50) * 0.3);

        return new TeamMatchStrength
        {
            Attack = attack,
            Midfield = midfield,
            Defense = defense,
            Goalkeeper = goalkeeper
        };
    }

    // ==========================
    // XG WITH FORMATION ADVANTAGE
    // ==========================
    private (double homeXg, double awayXg) CalculateXg(
        Squad home,
        Squad away,
        TeamMatchStrength homeStrength,
        TeamMatchStrength awayStrength)
    {
        double advantage =
            _formationMatchup.GetAdvantage(home.Formation, away.Formation);

        double homeXg =
            1.35
            + ((homeStrength.Attack - awayStrength.Defense) * 0.035)
            + ((homeStrength.Midfield - awayStrength.Midfield) * 0.020)
            - ((awayStrength.Goalkeeper - 70) * 0.015)
            + advantage;

        double awayXg =
            1.10
            + ((awayStrength.Attack - homeStrength.Defense) * 0.035)
            + ((awayStrength.Midfield - homeStrength.Midfield) * 0.020)
            - ((homeStrength.Goalkeeper - 70) * 0.015)
            - advantage;

        return (
            Math.Clamp(homeXg, 0.2, 4.5),
            Math.Clamp(awayXg, 0.2, 4.5)
        );
    }

    public MatchResult SimulateInstantMatch(Squad home, Squad away)
    {
        var homeStrength = CalculateTeamStrength(home);
        var awayStrength = CalculateTeamStrength(away);

        var (homeXg, awayXg) = CalculateXg(home, away, homeStrength, awayStrength);

        return new MatchResult
        {
            HomeGoals = GenerateGoals(homeXg),
            AwayGoals = GenerateGoals(awayXg)
        };
    }

    // ==========================
    // HELPERS
    // ==========================

    private int GenerateGoals(double xg)
    {
        double roll = _rng.NextDouble();

        if (xg < 0.5)
            return roll < 0.70 ? 0 : 1;

        if (xg < 1.5)
            return roll < 0.40 ? 0 : 1;

        if (xg < 2.5)
            return roll < 0.25 ? 1 : 2;

        if (xg < 3.5)
            return roll < 0.20 ? 1 : 3;

        return 2 + _rng.Next(3); // high-scoring chaos games
    }
    private Player GetRandomAttacker(Squad squad)
    {
        var attackers = squad.StartingXI
            .Where(p => IsAttacker(p.PreferredPosition))
            .ToList();

        return attackers.Count == 0
            ? squad.StartingXI[_rng.Next(squad.StartingXI.Count)]
            : attackers[_rng.Next(attackers.Count)];
    }

    private Player GetRandomPlayer(Squad home, Squad away)
    {
        var all = home.StartingXI.Concat(away.StartingXI).ToList();
        return all[_rng.Next(all.Count)];
    }

    private double SafeAvg(IEnumerable<double> values)
    {
        var list = values.ToList();
        return list.Count == 0 ? 50 : list.Average();
    }

    private bool IsAttacker(string pos) => pos is "ST" or "LW" or "RW" or "CF";
    private bool IsMidfielder(string pos) => pos is "CM" or "CDM" or "CAM" or "LM" or "RM";
    private bool IsDefender(string pos) => pos is "CB" or "LB" or "RB" or "LWB" or "RWB";
}