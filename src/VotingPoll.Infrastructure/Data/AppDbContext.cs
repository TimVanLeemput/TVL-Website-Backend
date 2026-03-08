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

        // Seed data — 52 weekly polls (ISO weeks 1–52, base year 2026)
        // Monday of ISO week 1 of 2026 = Dec 29, 2025
        DateTime weekOneStart = new DateTime(2025, 12, 29);
        // Weeks < CurrentWeek get a past ClosesAt (archived); current and future weeks stay open
        const int CurrentWeek = 10;

        (int Week, string Question, string[] Options)[] pollData =
        {
            (1,  "What is your favorite type of animal?",     new[] { "Cat", "Dog", "Bird", "Fish", "Horse", "Reptile", "Wild animals" }),
            (2,  "Which natural place do you enjoy most?",    new[] { "Forest", "Mountains", "Beach", "Lake", "Countryside", "Desert", "Waterfalls" }),
            (3,  "What kind of holiday sounds best?",         new[] { "Beach trip", "Mountain hiking", "City exploration", "Road trip", "Island getaway", "Camping", "Cultural tour" }),
            (4,  "Which meal do you enjoy the most?",         new[] { "Breakfast", "Brunch", "Lunch", "Dinner", "Late-night snack", "Dessert", "Street food" }),
            (5,  "Which season do you like most?",            new[] { "Spring", "Summer", "Autumn", "Winter", "Dry season", "Rainy season", "Late spring" }),
            (6,  "Which movie type do you enjoy most?",       new[] { "Comedy", "Action", "Drama", "Fantasy", "Science fiction", "Horror", "Documentary" }),
            (7,  "What kind of music do you listen to most?", new[] { "Pop", "Rock", "Electronic", "Hip hop", "Classical", "Jazz", "Indie / alternative" }),
            (8,  "Which pet would you choose?",               new[] { "Cat", "Dog", "Rabbit", "Bird", "Fish", "Reptile", "Hamster" }),
            (9,  "Which activity do you enjoy most?",         new[] { "Walking", "Running", "Cycling", "Swimming", "Gym workouts", "Yoga", "Team sports" }),
            (10, "What drink do you choose most often?",      new[] { "Water", "Coffee", "Tea", "Juice", "Soda", "Beer", "Wine" }),
            (11, "Which hobby sounds most fun?",              new[] { "Reading", "Gaming", "Cooking", "Photography", "Drawing", "Gardening", "DIY projects" }),
            (12, "Which book genre do you prefer?",           new[] { "Fantasy", "Mystery", "Science fiction", "Romance", "History", "Horror", "Non-fiction" }),
            (13, "What type of game do you enjoy?",           new[] { "Board games", "Video games", "Card games", "Puzzle games", "Party games", "Strategy games", "Arcade games" }),
            (14, "How do you relax best?",                    new[] { "Watching shows", "Walking outside", "Listening to music", "Talking with friends", "Reading", "Napping", "Meditation" }),
            (15, "Which weather do you enjoy most?",          new[] { "Warm sunny", "Cool breeze", "Rainy day", "Snowy day", "Stormy weather", "Mild cloudy", "Cool sunny day" }),
            (16, "Favorite snack?",                           new[] { "Chocolate", "Chips", "Fruit", "Nuts", "Cookies", "Ice cream", "Popcorn" }),
            (17, "How do you like to start the day?",         new[] { "Coffee", "Tea", "Exercise", "Shower", "Music", "Checking phone", "Quiet time" }),
            (18, "Ideal way to spend time with friends?",     new[] { "Dinner together", "Board games", "Outdoor activity", "Movie night", "Travel together", "Bar/café", "Online chat" }),
            (19, "Which outdoor sound do you like most?",     new[] { "Birds", "Ocean waves", "Rain", "Wind in trees", "Crackling fire", "Night insects", "Water stream" }),
            (20, "Which creative activity appeals most?",     new[] { "Drawing", "Writing", "Music", "Crafts", "Photography", "Cooking", "Design" }),
            (21, "Favorite way to travel?",                   new[] { "Car", "Train", "Plane", "Boat", "Bike", "Walking", "Camper van" }),
            (22, "How do you use your phone most?",           new[] { "Messaging", "Social media", "News", "Videos", "Music", "Games", "Work" }),
            (23, "Which animal is most fascinating?",         new[] { "Owl", "Fox", "Wolf", "Butterfly", "Octopus", "Elephant", "Dolphin" }),
            (24, "Favorite cuisine?",                         new[] { "Italian", "Japanese", "Mexican", "Indian", "French", "Thai", "Mediterranean" }),
            (25, "Best weekend activity?",                    new[] { "Relaxing at home", "Seeing friends", "Outdoor sports", "Travel", "Cooking", "Gaming", "Reading" }),
            (26, "Favorite ice cream flavor?",                new[] { "Vanilla", "Chocolate", "Strawberry", "Caramel", "Mint", "Cookie dough", "Pistachio" }),
            (27, "Favorite evening activity?",                new[] { "Watching movies", "Reading", "Gaming", "Going out", "Cooking", "Music", "Talking with friends" }),
            (28, "How do you drink coffee?",                  new[] { "Black", "With milk", "Latte", "Cappuccino", "Iced coffee", "Sweetened", "Espresso" }),
            (29, "Best wild animal to observe?",              new[] { "Deer", "Eagle", "Bear", "Whale", "Butterflies", "Wolves", "Dolphins" }),
            (30, "Favorite natural color palette?",           new[] { "Forest greens", "Ocean blues", "Sunset oranges", "Autumn reds", "Snow whites", "Desert tones", "Tropical greens" }),
            (31, "Favorite comfort food?",                    new[] { "Pizza", "Pasta", "Soup", "Burger", "Rice dishes", "Home-cooked meals", "Desserts" }),
            (32, "Where do you listen to music most?",        new[] { "At home", "In the car", "While working", "While exercising", "On walks", "At concerts", "At home with headphones" }),
            (33, "Favorite sport to watch?",                  new[] { "Football / Soccer", "Basketball", "Tennis", "Formula 1", "Olympics events", "Cycling", "Volleyball" }),
            (34, "Which landscape would you explore?",        new[] { "Tropical island", "Alpine mountains", "Northern forests", "Desert dunes", "Coastal cliffs", "Countryside", "Volcano areas" }),
            (35, "What cat personality do you prefer?",       new[] { "Very cuddly", "Playful", "Independent", "Calm", "Curious", "Lazy", "Adventurous cat" }),
            (36, "How do you like learning new things?",      new[] { "Books", "Videos", "Courses", "Practice", "Talking to people", "Podcasts", "Exploration" }),
            (37, "Favorite way to watch stories?",            new[] { "Movies", "TV series", "YouTube", "Streaming shows", "Documentaries", "Short videos", "Animation" }),
            (38, "Are you more?",                             new[] { "Early bird", "Night owl", "Balanced", "Depends on work", "Depends on season", "Random", "No idea" }),
            (39, "Favorite classic game?",                    new[] { "Chess", "Poker", "Monopoly", "Scrabble", "Uno", "Dominoes", "Checkers" }),
            (40, "What matters most on a trip?",              new[] { "Scenery", "Food", "People", "Culture", "Relaxation", "Adventure", "Price" }),
            (41, "Favorite sky moment?",                      new[] { "Sunrise", "Sunset", "Starry night", "Full moon", "Storm clouds", "Clear blue sky", "Northern lights" }),
            (42, "Favorite ocean animal?",                    new[] { "Dolphin", "Whale", "Shark", "Sea turtle", "Octopus", "Seal", "Jellyfish" }),
            (43, "Favorite fruit?",                           new[] { "Apple", "Banana", "Strawberry", "Mango", "Orange", "Pineapple", "Peach" }),
            (44, "Favorite hot drink?",                       new[] { "Coffee", "Tea", "Hot chocolate", "Matcha", "Herbal tea", "Spiced drinks", "Mate" }),
            (45, "Best quiet activity?",                      new[] { "Reading", "Walking", "Listening to music", "Drawing", "Meditation", "Journaling", "Napping" }),
            (46, "What makes a great friend?",                new[] { "Loyalty", "Humor", "Honesty", "Kindness", "Support", "Shared interests", "Good listener" }),
            (47, "Favorite creative style?",                  new[] { "Minimal", "Colorful", "Nature inspired", "Abstract", "Realistic", "Vintage", "Modern" }),
            (48, "Dream place to stay?",                      new[] { "Beach house", "Mountain cabin", "City loft", "Forest cabin", "Island bungalow", "Countryside farm", "Boat house" }),
            (49, "Favorite dessert?",                         new[] { "Chocolate cake", "Ice cream", "Cheesecake", "Cookies", "Fruit tart", "Brownies", "Pancakes" }),
            (50, "Favorite outdoor activity?",                new[] { "Hiking", "Swimming", "Picnic", "Photography", "Cycling", "Bird watching", "Camping" }),
            (51, "What improves life most?",                  new[] { "Health", "Good friends", "Nature", "Free time", "Learning", "Creativity", "Travel" }),
            (52, "What brings you the most joy?",             new[] { "Family", "Friends", "Nature", "Achievements", "Creativity", "Helping others", "Peaceful moments" }),
        };

        List<Poll> seedPolls = new List<Poll>();
        List<PollOption> seedOptions = new List<PollOption>();
        int optionId = 1;

        foreach ((int week, string question, string[] options) in pollData)
        {
            DateTime createdAt = weekOneStart.AddDays((week - 1) * 7);
            seedPolls.Add(new Poll
            {
                Id = week,
                Title = question,
                WeekNumber = week,
                CreatedAt = createdAt,
                ClosesAt = week < CurrentWeek ? createdAt.AddDays(7) : (DateTime?)null,
            });

            foreach (string option in options)
            {
                seedOptions.Add(new PollOption { Id = optionId++, PollId = week, PollOptionName = option });
            }
        }

        modelBuilder.Entity<Poll>().HasData(seedPolls);
        modelBuilder.Entity<PollOption>().HasData(seedOptions);

//
        // Index
        // -- "For the Vote table, create an index on the combination of PollId and
        //     UserId, and make sure that combination is unique - no duplicates allowed. This also improved query performance" -- 
        modelBuilder.Entity<Vote>().HasIndex(v => new { v.PollId, v.UserId }).IsUnique();
    }
}