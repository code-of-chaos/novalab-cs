using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class SoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSoftDeleted",
                table: "TrackedStreamSubjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSoftDeleted",
                table: "TrackedStreamSubjectComponents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSoftDeleted",
                table: "TrackedStreamSubjects");

            migrationBuilder.DropColumn(
                name: "IsSoftDeleted",
                table: "TrackedStreamSubjectComponents");
        }
    }
}
