using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class Renaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwitchGameTitleToIdCache");

            migrationBuilder.DropTable(
                name: "TwitchManagedRewardRedemptions");

            migrationBuilder.DropTable(
                name: "TwitchStreamSubject");

            migrationBuilder.DropTable(
                name: "TwitchManagedRewards");

            migrationBuilder.CreateTable(
                name: "GameTitleToTwitchIds",
                columns: table => new
                {
                    NovaLabName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TwitchTitleId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TwitchTitleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TwitchTitleBoxArtUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TwitchTitleIgdbId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTitleToTwitchIds", x => x.NovaLabName);
                });

            migrationBuilder.CreateTable(
                name: "StreamSubjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TwitchGameId = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: true),
                    TwitchBroadcastLanguage = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    TwitchTitle = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    TwitchTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSoftDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreamSubjects_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackedRewards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TwitchRewardId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TemplatePerRedemption = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TemplateTotal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastCleared = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedRewards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedRewards_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackedRewardRedemptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackedRewardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsSoftDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedRewardRedemptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedRewardRedemptions_TrackedRewards_TrackedRewardId",
                        column: x => x.TrackedRewardId,
                        principalTable: "TrackedRewards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StreamSubjects_UserId",
                table: "StreamSubjects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedRewardRedemptions_TrackedRewardId",
                table: "TrackedRewardRedemptions",
                column: "TrackedRewardId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedRewards_UserId",
                table: "TrackedRewards",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameTitleToTwitchIds");

            migrationBuilder.DropTable(
                name: "StreamSubjects");

            migrationBuilder.DropTable(
                name: "TrackedRewardRedemptions");

            migrationBuilder.DropTable(
                name: "TrackedRewards");

            migrationBuilder.CreateTable(
                name: "TwitchGameTitleToIdCache",
                columns: table => new
                {
                    NovaLabName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TwitchTitleBoxArtUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TwitchTitleId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TwitchTitleIgdbId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TwitchTitleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitchGameTitleToIdCache", x => x.NovaLabName);
                });

            migrationBuilder.CreateTable(
                name: "TwitchManagedRewards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastCleared = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TemplatePerRedemption = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TemplateTotal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TwitchRewardId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitchManagedRewards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwitchManagedRewards_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TwitchStreamSubject",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TwitchBroadcastLanguage = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    TwitchGameId = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: true),
                    TwitchTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwitchTitle = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitchStreamSubject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwitchStreamSubject_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TwitchManagedRewardRedemptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TwitchManagedRewardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_TwitchManagedRewards_UserId",
                table: "TwitchManagedRewards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TwitchStreamSubject_UserId",
                table: "TwitchStreamSubject",
                column: "UserId");
        }
    }
}
