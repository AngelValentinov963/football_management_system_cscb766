using football_management_system_cscb.Models;
using Microsoft.EntityFrameworkCore;

namespace football_management_system_cscb.Data
{
    public class FootballDbContext : DbContext
    {
        public FootballDbContext(DbContextOptions<FootballDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }


        // Tables
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<FootballMatch> FootballMatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TABLE MAPPINGS (VERY IMPORTANT)

            modelBuilder.Entity<Team>().ToTable("team");
            modelBuilder.Entity<Player>().ToTable("player");
            modelBuilder.Entity<FootballMatch>().ToTable("football_match");

            // OPTIONAL: if your PKs are not convention-based

            modelBuilder.Entity<Team>()
                .HasKey(t => t.TeamId);

            modelBuilder.Entity<Player>()
                .HasKey(p => p.PlayerId);

            modelBuilder.Entity<FootballMatch>()
                .HasKey(m => m.MatchId);

            // OPTIONAL RELATIONSHIPS (safe basic setup)

            modelBuilder.Entity<Player>()
                .HasOne<Team>()
                .WithMany()
                .HasForeignKey(p => p.TeamId);

            modelBuilder.Entity<FootballMatch>()
                .HasOne<Team>()
                .WithMany()
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FootballMatch>()
                .HasOne<Team>()
                .WithMany()
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}