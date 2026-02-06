using Microsoft.EntityFrameworkCore;
using VotingPoll.API.Entities;

namespace VotingPoll.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Poll> Polls { get; set; }
    public DbSet<PollOption> PollOptions { get; set; }
    public DbSet<Vote> Votes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Poll>().HasData(new Poll { Id = 1, Title = "Test Poll" });
    }
}
