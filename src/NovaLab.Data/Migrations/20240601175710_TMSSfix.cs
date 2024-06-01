using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class TMSSfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ManagedStreamSubjects_SelectedManagedStreamSubje~",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ManagedStreamSubjects",
                table: "ManagedStreamSubjects");

            migrationBuilder.DropColumn(
                name: "Selected",
                table: "ManagedStreamSubjects");

            migrationBuilder.RenameTable(
                name: "ManagedStreamSubjects",
                newName: "TwitchManagedStreamSubjects");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TwitchManagedStreamSubjects",
                table: "TwitchManagedStreamSubjects",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_TwitchManagedStreamSubjects_SelectedManagedStrea~",
                table: "AspNetUsers",
                column: "SelectedManagedStreamSubjectId",
                principalTable: "TwitchManagedStreamSubjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_TwitchManagedStreamSubjects_SelectedManagedStrea~",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TwitchManagedStreamSubjects",
                table: "TwitchManagedStreamSubjects");

            migrationBuilder.RenameTable(
                name: "TwitchManagedStreamSubjects",
                newName: "ManagedStreamSubjects");

            migrationBuilder.AddColumn<bool>(
                name: "Selected",
                table: "ManagedStreamSubjects",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ManagedStreamSubjects",
                table: "ManagedStreamSubjects",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ManagedStreamSubjects_SelectedManagedStreamSubje~",
                table: "AspNetUsers",
                column: "SelectedManagedStreamSubjectId",
                principalTable: "ManagedStreamSubjects",
                principalColumn: "Id");
        }
    }
}
