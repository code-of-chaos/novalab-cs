using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class TwitchManagedRewardRedemption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomTwitchRedemptions_AspNetUsers_UserId",
                table: "CustomTwitchRedemptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomTwitchRedemptions",
                table: "CustomTwitchRedemptions");

            migrationBuilder.RenameTable(
                name: "CustomTwitchRedemptions",
                newName: "TwitchManagedRewards");

            migrationBuilder.RenameIndex(
                name: "IX_CustomTwitchRedemptions_UserId",
                table: "TwitchManagedRewards",
                newName: "IX_TwitchManagedRewards_UserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCleared",
                table: "TwitchManagedRewards",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TwitchManagedRewards",
                table: "TwitchManagedRewards",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TwitchManagedRewardRedemptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TwitchManagedRewardId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitchManagedRewardRedemptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwitchManagedRewardRedemptions_TwitchManagedRewards_TwitchManagedRewardId",
                        column: x => x.TwitchManagedRewardId,
                        principalTable: "TwitchManagedRewards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TwitchManagedRewardRedemptions_TwitchManagedRewardId",
                table: "TwitchManagedRewardRedemptions",
                column: "TwitchManagedRewardId");

            migrationBuilder.AddForeignKey(
                name: "FK_TwitchManagedRewards_AspNetUsers_UserId",
                table: "TwitchManagedRewards",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TwitchManagedRewards_AspNetUsers_UserId",
                table: "TwitchManagedRewards");

            migrationBuilder.DropTable(
                name: "TwitchManagedRewardRedemptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TwitchManagedRewards",
                table: "TwitchManagedRewards");

            migrationBuilder.DropColumn(
                name: "LastCleared",
                table: "TwitchManagedRewards");

            migrationBuilder.RenameTable(
                name: "TwitchManagedRewards",
                newName: "CustomTwitchRedemptions");

            migrationBuilder.RenameIndex(
                name: "IX_TwitchManagedRewards_UserId",
                table: "CustomTwitchRedemptions",
                newName: "IX_CustomTwitchRedemptions_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomTwitchRedemptions",
                table: "CustomTwitchRedemptions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomTwitchRedemptions_AspNetUsers_UserId",
                table: "CustomTwitchRedemptions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
