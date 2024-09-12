using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAfterRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackedStreamSubjectComponents");

            migrationBuilder.DropTable(
                name: "TrackedStreamSubjects");

            migrationBuilder.CreateTable(
                name: "TwitchManagedRewards",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    TwitchRewardId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    TemplatePerRedemption = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TemplateTotal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastCleared = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    Id = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    TwitchGameId = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: true),
                    TwitchBroadcastLanguage = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    TwitchTitle = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    TwitchTags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSoftDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    Id = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    TwitchManagedRewardId = table.Column<string>(type: "nvarchar(26)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsSoftDeleted = table.Column<bool>(type: "bit", nullable: false)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwitchManagedRewardRedemptions");

            migrationBuilder.DropTable(
                name: "TwitchStreamSubject");

            migrationBuilder.DropTable(
                name: "TwitchManagedRewards");

            migrationBuilder.CreateTable(
                name: "TrackedStreamSubjects",
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
                    table.PrimaryKey("PK_TrackedStreamSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedStreamSubjects_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackedStreamSubjectComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentStyling = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ComponentText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TrackedStreamSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedStreamSubjectComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedStreamSubjectComponents_TrackedStreamSubjects_Id",
                        column: x => x.Id,
                        principalTable: "TrackedStreamSubjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackedStreamSubjectComponents_Id",
                table: "TrackedStreamSubjectComponents",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackedStreamSubjects_UserId",
                table: "TrackedStreamSubjects",
                column: "UserId");
        }
    }
}
