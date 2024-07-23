using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class TwitchGameCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TwitchGameTitleToIdCache",
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
                    table.PrimaryKey("PK_TwitchGameTitleToIdCache", x => x.NovaLabName);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TwitchGameTitleToIdCache");
        }
    }
}
