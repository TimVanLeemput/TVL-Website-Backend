using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingPoll.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDatesTo2026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 29, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2026, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 3, 23, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 4, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 4, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 7, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 9, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 10, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 10, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 10, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 11, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 11, 16, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 12, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { null, new DateTime(2026, 12, 21, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 12, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 1, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 21, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 7, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 8, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 7, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 18, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 9, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 10, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 29, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 6, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 11, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 24, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Polls",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "ClosesAt", "CreatedAt" },
                values: new object[] { new DateTime(2025, 12, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}
