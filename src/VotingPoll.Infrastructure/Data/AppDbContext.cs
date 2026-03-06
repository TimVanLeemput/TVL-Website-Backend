using Microsoft.EntityFrameworkCore;
using VotingPoll.Core.Entities;
using VotingPoll.Core.Entities.Authentication;

namespace VotingPoll.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Poll> Polls { get; set; }
    public DbSet<PollOption> PollOptions { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

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

        modelBuilder.Entity<User>().HasMany(u => u.RefreshTokens).WithOne(rt => rt.User)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<User>().Property(r => r.Role)
            .HasConversion<string>(); // Allows enum string to be foldout as enum

        // Seed data
        modelBuilder.Entity<Poll>().HasData(
            new Poll { Id = 1, Title = "What's your favorite theme?", CreatedAt = new DateTime(2025, 1, 1) }
        );

        modelBuilder.Entity<PollOption>().HasData(
            new PollOption { Id = 1, PollOptionName = "Stars", PollId = 1 },
            new PollOption { Id = 2, PollOptionName = "Sea", PollId = 1 },
            new PollOption { Id = 3, PollOptionName = "Brutalist", PollId = 1 },
            new PollOption { Id = 4, PollOptionName = "Bugs", PollId = 1 },
            new PollOption { Id = 5, PollOptionName = "Mushrooms", PollId = 1 },
            new PollOption { Id = 6, PollOptionName = "Cats", PollId = 1 }
        );
//
        // Index
        // -- "For the Vote table, create an index on the combination of PollId and
        //     UserId, and make sure that combination is unique - no duplicates allowed. This also improved query performance" -- 
        modelBuilder.Entity<Vote>().HasIndex(v => new { v.PollId, v.UserId }).IsUnique();
    }
}