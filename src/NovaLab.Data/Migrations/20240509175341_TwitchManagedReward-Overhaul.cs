using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class TwitchManagedRewardOverhaul : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "CustomTwitchRedemptions");

            migrationBuilder.RenameColumn(
                name: "TwitchRedemptionId",
                table: "CustomTwitchRedemptions",
                newName: "OutputTemplatePerReward");

            migrationBuilder.RenameColumn(
                name: "RedemptionType",
                table: "CustomTwitchRedemptions",
                newName: "PointsCost");

            migrationBuilder.RenameColumn(
                name: "Points",
                table: "CustomTwitchRedemptions",
                newName: "HasPrompt");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CustomTwitchRedemptions",
                newName: "OutputTemplate");

            migrationBuilder.AddColumn<string>(
                name: "RewardId",
                table: "CustomTwitchRedemptions",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CustomTwitchRedemptions",
                type: "TEXT",
                maxLength: 45,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RewardId",
                table: "CustomTwitchRedemptions");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "CustomTwitchRedemptions");

            migrationBuilder.RenameColumn(
                name: "PointsCost",
                table: "CustomTwitchRedemptions",
                newName: "RedemptionType");

            migrationBuilder.RenameColumn(
                name: "OutputTemplatePerReward",
                table: "CustomTwitchRedemptions",
                newName: "TwitchRedemptionId");

            migrationBuilder.RenameColumn(
                name: "OutputTemplate",
                table: "CustomTwitchRedemptions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "HasPrompt",
                table: "CustomTwitchRedemptions",
                newName: "Points");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CustomTwitchRedemptions",
                type: "TEXT",
                nullable: true);
        }
    }
}
