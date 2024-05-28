using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class TwitchFollowerGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TwitchFollowerGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DailyGoal = table.Column<int>(type: "int", nullable: false),
                    CustomCssStyling = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitchFollowerGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwitchFollowerGoals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TwitchNewFollowers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GoalId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TimeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FollowerTwitchUserId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitchNewFollowers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwitchNewFollowers_TwitchFollowerGoals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "TwitchFollowerGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TwitchFollowerGoals_UserId",
                table: "TwitchFollowerGoals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TwitchNewFollowers_GoalId",
                table: "TwitchNewFollowers",
                column: "GoalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwitchNewFollowers");

            migrationBuilder.DropTable(
                name: "TwitchFollowerGoals");
        }
    }
}
