using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class FKsBetweenUserAndFollowerGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TwitchFollowerGoals_AspNetUsers_UserId",
                table: "TwitchFollowerGoals");

            migrationBuilder.DropIndex(
                name: "IX_TwitchFollowerGoals_UserId",
                table: "TwitchFollowerGoals");

            migrationBuilder.UpdateData(
                table: "TwitchFollowerGoals",
                keyColumn: "UserId",
                keyValue: null,
                column: "UserId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TwitchFollowerGoals",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TwitchFollowerGoals_UserId",
                table: "TwitchFollowerGoals",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TwitchBroadcasterId",
                table: "AspNetUsers",
                column: "TwitchBroadcasterId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TwitchFollowerGoals_AspNetUsers_UserId",
                table: "TwitchFollowerGoals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TwitchFollowerGoals_AspNetUsers_UserId",
                table: "TwitchFollowerGoals");

            migrationBuilder.DropIndex(
                name: "IX_TwitchFollowerGoals_UserId",
                table: "TwitchFollowerGoals");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TwitchBroadcasterId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TwitchFollowerGoals",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TwitchFollowerGoals_UserId",
                table: "TwitchFollowerGoals",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TwitchFollowerGoals_AspNetUsers_UserId",
                table: "TwitchFollowerGoals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
