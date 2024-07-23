using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrackedStreamSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackedStreamSubjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TwitchGameId = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: true),
                    TwitchBroadcastLanguage = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    TwitchTitle = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    TwitchTags = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    TrackedStreamSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponentText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ComponentStyling = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedStreamSubjectComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedStreamSubjectComponents_TrackedStreamSubjects_TrackedStreamSubjectId",
                        column: x => x.TrackedStreamSubjectId,
                        principalTable: "TrackedStreamSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackedStreamSubjectComponents_TrackedStreamSubjectId",
                table: "TrackedStreamSubjectComponents",
                column: "TrackedStreamSubjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackedStreamSubjects_UserId",
                table: "TrackedStreamSubjects",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackedStreamSubjectComponents");

            migrationBuilder.DropTable(
                name: "TrackedStreamSubjects");
        }
    }
}
