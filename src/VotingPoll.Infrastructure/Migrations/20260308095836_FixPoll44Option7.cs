using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingPoll.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPoll44Option7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 308,
                column: "PollOptionName",
                value: "Mate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 308,
                column: "PollOptionName",
                value: "Volleyball");
        }
    }
}
