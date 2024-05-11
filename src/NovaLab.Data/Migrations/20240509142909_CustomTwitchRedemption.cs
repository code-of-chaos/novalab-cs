using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomTwitchRedemption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomTwitchRedemptions",
                columns: table => new
                {
                    key = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RedemptionType = table.Column<int>(type: "INTEGER", nullable: false),
                    TwitchRedemptionId = table.Column<string>(type: "TEXT", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomTwitchRedemptions", x => x.key);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomTwitchRedemptions");
        }
    }
}
