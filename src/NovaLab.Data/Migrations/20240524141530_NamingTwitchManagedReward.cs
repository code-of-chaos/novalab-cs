using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class NamingTwitchManagedReward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OutputTemplate",
                table: "TwitchManagedRewards",
                newName: "OutputTemplatePerRedemption");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OutputTemplatePerRedemption",
                table: "TwitchManagedRewards",
                newName: "OutputTemplate");
        }
    }
}
