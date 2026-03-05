IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'VotingPollDb')
BEGIN
    CREATE DATABASE VotingPollDb;
END
