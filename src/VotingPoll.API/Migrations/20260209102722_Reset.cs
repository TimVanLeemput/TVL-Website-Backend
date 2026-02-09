using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingPoll.API.Migrations
{
    /// <inheritdoc />
    public partial class Reset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosesAt",
                table: "PollOptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosesAt",
                table: "PollOptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClosesAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "ClosesAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "PollOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClosesAt",
                value: null);
        }
    }
}
