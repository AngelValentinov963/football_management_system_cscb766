using Microsoft.EntityFrameworkCore;

public class football_management_system_cscbContext(DbContextOptions<football_management_system_cscbContext> options) : DbContext(options)
{
    public DbSet<football_management_system_cscb.Models.Team> Team { get; set; } = default!;
}
