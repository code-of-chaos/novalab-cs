using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class TwitchBroadcasterId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TwitchBroadcasterId",
                table: "AspNetUsers",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwitchBroadcasterId",
                table: "AspNetUsers");
        }
    }
}
