using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingPoll.API.Migrations
{
    /// <inheritdoc />
    public partial class _128763 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_PollOptions_PollOptionId",
                table: "Votes");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_PollOptions_PollOptionId",
                table: "Votes",
                column: "PollOptionId",
                principalTable: "PollOptions",
                principalColumn: "PollId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_PollOptions_PollOptionId",
                table: "Votes");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_PollOptions_PollOptionId",
                table: "Votes",
                column: "PollOptionId",
                principalTable: "PollOptions",
                principalColumn: "PollId");
        }
    }
}
