using football_management_system_cscb.Data;
using football_management_system_cscb.Models;
using football_management_system_cscb.Models.Formation;
using Microsoft.EntityFrameworkCore;

namespace football_management_system_cscb.Services
{
    public class SquadService
    {
        private readonly FootballDbContext _context;

        public SquadService(FootballDbContext context)
        {
            _context = context;
        }

        public Squad GetSquad(int? teamId, Formation formation)
        {
            var team = _context.Teams
                .Include(t => t.Players)
                .FirstOrDefault(t => t.TeamId == teamId);

            if (team == null)
                throw new Exception($"Team {teamId} not found");

            return BuildSquad(team, formation);
        }

        private Squad BuildSquad(Team team, Formation formation)
        {
            var squad = new Squad(team, formation);

            var players = team.Players
                .OrderByDescending(p => p.OverallRating ?? 50)
                .ToList();

            var gks = players.Where(p => p.PreferredPosition == "GK").Take(formation.Goalkeepers);
            var defs = players.Where(p => p.PreferredPosition is "CB" or "LB" or "RB").Take(formation.Defenders);
            var mids = players.Where(p => p.PreferredPosition is "CM" or "CDM" or "CAM" or "LM" or "RM").Take(formation.Midfielders);
            var atts = players.Where(p => p.PreferredPosition is "ST" or "LW" or "RW" or "CF").Take(formation.Attackers);

            squad.StartingXI = gks
                .Concat(defs)
                .Concat(mids)
                .Concat(atts)
                .ToList();

            var used = new HashSet<int>(squad.StartingXI.Select(p => p.PlayerId));

            squad.Bench = players
                .Where(p => !used.Contains(p.PlayerId))
                .Take(7)
                .ToList();

            return squad;
        }
    }
}