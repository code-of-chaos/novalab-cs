using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Data.Migrations
{
    /// <inheritdoc />
    public partial class TwitchAccessToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TwitchAccessToken",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwitchAccessToken",
                table: "AspNetUsers");
        }
    }
}
