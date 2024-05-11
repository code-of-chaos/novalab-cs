using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class VirtualCustomTwitchRedemption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CustomTwitchRedemptions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_CustomTwitchRedemptions_UserId",
                table: "CustomTwitchRedemptions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomTwitchRedemptions_AspNetUsers_UserId",
                table: "CustomTwitchRedemptions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomTwitchRedemptions_AspNetUsers_UserId",
                table: "CustomTwitchRedemptions");

            migrationBuilder.DropIndex(
                name: "IX_CustomTwitchRedemptions_UserId",
                table: "CustomTwitchRedemptions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CustomTwitchRedemptions",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
