using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VotingPoll.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedCreateVote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Polls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalVotes = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosesAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PollOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PollOptionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PollId = table.Column<int>(type: "int", nullable: false),
                    TotalVotes = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosesAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollOptions_Polls_PollId",
                        column: x => x.PollId,
                        principalTable: "Polls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PollOptionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VotedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PollId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_PollOptions_PollOptionId",
                        column: x => x.PollOptionId,
                        principalTable: "PollOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votes_Polls_PollId",
                        column: x => x.PollId,
                        principalTable: "Polls",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Polls",
                columns: new[] { "Id", "ClosesAt", "CreatedAt", "Title", "TotalVotes" },
                values: new object[] { 1, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "What's your favorite color?", 0 });

            migrationBuilder.InsertData(
                table: "PollOptions",
                columns: new[] { "Id", "ClosesAt", "CreatedAt", "PollId", "PollOptionName", "TotalVotes" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Blue", 0 },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Red", 0 },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Green", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollOptions_PollId",
                table: "PollOptions",
                column: "PollId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_PollId_UserId",
                table: "Votes",
                columns: new[] { "PollId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_PollOptionId",
                table: "Votes",
                column: "PollOptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "PollOptions");

            migrationBuilder.DropTable(
                name: "Polls");
        }
    }
}
