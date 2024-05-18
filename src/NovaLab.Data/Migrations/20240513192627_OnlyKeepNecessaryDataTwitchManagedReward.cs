using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class OnlyKeepNecessaryDataTwitchManagedReward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPrompt",
                table: "TwitchManagedRewards");

            migrationBuilder.DropColumn(
                name: "PointsCost",
                table: "TwitchManagedRewards");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "TwitchManagedRewards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasPrompt",
                table: "TwitchManagedRewards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PointsCost",
                table: "TwitchManagedRewards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TwitchManagedRewards",
                type: "TEXT",
                maxLength: 45,
                nullable: false,
                defaultValue: "");
        }
    }
}
