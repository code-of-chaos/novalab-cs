using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Data.Migrations
{
    /// <inheritdoc />
    public partial class TwitchBroadcasterId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwitchAccessToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TwitchRefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TwitchRefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "TwitchBroadcasterId",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwitchBroadcasterId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "TwitchAccessToken",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TwitchRefreshToken",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TwitchRefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
