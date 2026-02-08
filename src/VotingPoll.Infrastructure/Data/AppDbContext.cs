using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.Entities;

namespace VotingPoll.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Poll> Polls { get; set; }
    public DbSet<PollOption> PollOptions { get; set; }
    public DbSet<Vote> Votes { get; set; }

    // Fluent API
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.Poll)
            .WithMany()
            .HasForeignKey(v => v.PollId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Vote>()
            .HasOne(v => v.PollOption)
            .WithMany(po => po.AllVotes)
            .HasForeignKey(v => v.PollOptionId)
            .OnDelete(DeleteBehavior.Cascade);


        
        // Seed data
        modelBuilder.Entity<Poll>().HasData(
            new Poll { Id = 1, Title = "What's your favorite color?", CreatedAt = new DateTime(2025, 1, 1) }
        );

        modelBuilder.Entity<PollOption>().HasData(
            new PollOption { Id = 1, PollOptionName = "Blue", PollId = 1 },
            new PollOption { Id = 2, PollOptionName = "Red", PollId = 1 },
            new PollOption { Id = 3, PollOptionName = "Green", PollId = 1 }
        );

        // Index
        // -- "For the Vote table, create an index on the combination of PollId and
        //     UserId, and make sure that combination is unique - no duplicates allowed." -- 
        modelBuilder.Entity<Vote>().HasIndex(v => new {v.PollId, v.UserId }).IsUnique();
    }
}