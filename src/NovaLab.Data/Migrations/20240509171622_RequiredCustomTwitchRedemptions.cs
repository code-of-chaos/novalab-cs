using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequiredCustomTwitchRedemptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "key",
                table: "CustomTwitchRedemptions",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CustomTwitchRedemptions",
                newName: "key");
        }
    }
}
