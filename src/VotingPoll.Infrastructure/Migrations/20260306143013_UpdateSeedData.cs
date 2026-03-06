using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VotingPoll.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "PollOptionName",
                value: "Stars");

            migrationBuilder.UpdateData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "PollOptionName",
                value: "Sea");

            migrationBuilder.UpdateData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "PollOptionName",
                value: "Brutalist");

            migrationBuilder.InsertData(
                table: "PollOptions",
                columns: new[] { "Id", "CreatedAt", "PollId", "PollOptionName", "TotalVotes" },
                values: new object[,]
                {
                    { 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Bugs", 0 },
                    { 5, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Mushrooms", 0 },
                    { 6, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Cats", 0 }
                });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "PollOptionName",
                value: "Blue");

            migrationBuilder.UpdateData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "PollOptionName",
                value: "Red");

            migrationBuilder.UpdateData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "PollOptionName",
                value: "Green");

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
