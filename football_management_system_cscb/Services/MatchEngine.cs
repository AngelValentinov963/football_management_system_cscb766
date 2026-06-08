using football_management_system_cscb.Models;
using football_management_system_cscb.Models.MatchSimulation;
using football_management_system_cscb.Services;


public class MatchEngine
{
    private readonly Random _rng = new();
    private readonly TeamService _rating;
    // ==========================
    // START MATCH
    // ==========================
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
            AwayActivePlayerIds = new List<int>()
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

        // INIT ACTIVE PLAYERS (IMPORTANT FIX)
        if (!state.HomeActivePlayerIds.Any())
            state.HomeActivePlayerIds = home.StartingXI.Select(p => p.PlayerId).ToList();

        if (!state.AwayActivePlayerIds.Any())
            state.AwayActivePlayerIds = away.StartingXI.Select(p => p.PlayerId).ToList();


        state.CurrentMinute++;

        var homeAttack = GetAttackStrength(home, state.HomeActivePlayerIds);
        var awayAttack = GetAttackStrength(away, state.AwayActivePlayerIds);

        var homeDefense = GetDefenseStrength(home, state.HomeActivePlayerIds);
        var awayDefense = GetDefenseStrength(away, state.AwayActivePlayerIds);

        var homeChance = homeAttack / (homeAttack + awayDefense + 1);
        var awayChance = awayAttack / (awayAttack + homeDefense + 1);

        // NOW momentum works
        state.Momentum += (homeChance - awayChance) * 5;
        state.Momentum = Math.Clamp(state.Momentum, -100, 100);

        ProcessGoalChance(state, home, away, homeChance, awayChance);
        ProcessRandomEvents(state, home, away);
        ProcessCommentary(state, home, away);

        if (state.Momentum > 60)
        {
            state.Events.Add(new CommentaryEvent
            {
                Minute = state.CurrentMinute,
                Description = $"{home.Team.Name} are heavily pressing!"
            });
        }
        else if (state.Momentum < -60)
        {
            state.Events.Add(new CommentaryEvent
            {
                Minute = state.CurrentMinute,
                Description = $"{away.Team.Name} are dominating possession"
            });
        }

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
    private CommentaryEvent CreateCommentary(int minute, string text)
    {
        return new CommentaryEvent
        {
            Minute = minute,
            Description = text
        };
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
                TeamName = home.StartingXI.Contains(player)
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
        else if (roll < 0.05)
        {
            var team = _rng.Next(2) == 0 ? home : away;

            state.Events.Add(new CornerEvent
            {
                Minute = state.CurrentMinute,
                Description = $"🚩 Corner kick for {team.Team.Name}"
            });
        }
        else if (roll < 0.07)
        {
            var team = _rng.Next(2) == 0 ? home : away;

            state.Events.Add(new ThrowInEvent
            {
                Minute = state.CurrentMinute,
                Description = $"Throw-in for {team.Team.Name}"
            });
        }
        else if (roll < 0.10)
        {
            var team = _rng.Next(2) == 0 ? home : away;

            state.Events.Add(new ShotEvent
            {
                Minute = state.CurrentMinute,
                Description = $"Shot on goal by {team.Team.Name}"
            });
        }
    }
    // ==========================
    // RED CARD FIXED
    // ==========================
    private void HandleRedCard(
        MatchState state,
        Squad home,
        Squad away,
        Player player)
    {
        var isHome = home.StartingXI.Any(p => p.PlayerId == player.PlayerId);

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

    private void ProcessCommentary(MatchState state, Squad home, Squad away)
    {
        int minutesSinceLast = state.CurrentMinute - state.LastCommentaryMinute;

        // ⛔ enforce max silence = 3 minutes
        if (minutesSinceLast < 3 && _rng.NextDouble() > 0.25)
            return;

        double roll = _rng.NextDouble();

        bool homeDominating = state.Momentum > 40;
        bool awayDominating = state.Momentum < -40;

        string desc = null;

        if (roll < 0.05)
        {
            desc = homeDominating
                ? $"🔥 {home.Team.Name} are completely in control of this match!"
                : awayDominating
                    ? $"⚡ {away.Team.Name} are dictating the tempo right now!"
                    : "Midfield battle continues with neither side taking control";
        }
        else if (roll < 0.10)
        {
            desc = homeDominating
                ? $"{home.Team.Name} are piling on the pressure!"
                : awayDominating
                    ? $"{away.Team.Name} are pushing forward with confidence!"
                    : "Both teams are probing for an opening";
        }
        else if (roll < 0.15)
        {
            desc = state.CurrentMinute < 20
                ? "Early stages, both teams still feeling each other out"
                : state.CurrentMinute < 70
                    ? "The tempo has dropped slightly as fatigue starts to show"
                    : "We're entering the decisive phase of the match!";
        }
        else if (roll < 0.20)
        {
            desc = (homeDominating || awayDominating)
                ? "The crowd is reacting to the intense pressure on the pitch!"
                : "🔥 The atmosphere inside the stadium is absolutely electric!";
        }
        else if (roll < 0.23)
        {
            desc = $"⏱ {state.CurrentMinute}' - The tension is building";
        }

        if (desc != null)
        {
            state.Events.Add(CreateCommentary(state.CurrentMinute, desc));
            state.LastCommentaryMinute = state.CurrentMinute;
        }
    }
    // ==========================
    // STRENGTH
    // ==========================
    private double GetAttackStrength(Squad squad, List<int> activeIds)
    {
        var players = squad.StartingXI
            .Where(p => activeIds.Contains(p.PlayerId))
            .Where(p => IsAttacker(p.PreferredPosition))
            .ToList();

        if (!players.Any())
            return 40;

        return players.Average(p =>
            (p.Shooting + p.Pace + p.Dribbling + (p.OverallRating ?? 50)) / 4.0);
    }

    private double GetDefenseStrength(Squad squad, List<int> activeIds)
    {
        var players = squad.StartingXI
            .Where(p => activeIds.Contains(p.PlayerId))
            .Where(p => IsDefender(p.PreferredPosition))
            .ToList();

        if (!players.Any())
            return 40;

        return players.Average(p =>
            (p.Defense + p.Stamina + (p.OverallRating ?? 50)) / 3.0);
    }

    // ==========================
    // HELPERS
    // ==========================
    private Player GetRandomAttacker(Squad squad)
    {
        var attackers = squad.StartingXI
            .Where(p => IsAttacker(p.PreferredPosition))
            .ToList();

        return attackers[_rng.Next(attackers.Count)];
    }

    private Player GetRandomPlayer(Squad home, Squad away)
    {
        var all = home.StartingXI.Concat(away.StartingXI).ToList();
        return all[_rng.Next(all.Count)];
    }

    private bool IsAttacker(string pos)
        => pos is "ST" or "LW" or "RW" or "CF";

    private bool IsDefender(string pos)
        => pos is "CB" or "LB" or "RB" or "LWB" or "RWB";
}


