using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class TwitchTokensData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
