using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddManagedStreamSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SelectedManagedStreamSubjectId",
                table: "AspNetUsers",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "ManagedStreamSubjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Selected = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SelectionName = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ObsSubjectTitle = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TwitchSubjectTitle = table.Column<string>(type: "varchar(140)", maxLength: 140, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagedStreamSubjects", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SelectedManagedStreamSubjectId",
                table: "AspNetUsers",
                column: "SelectedManagedStreamSubjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ManagedStreamSubjects_SelectedManagedStreamSubje~",
                table: "AspNetUsers",
                column: "SelectedManagedStreamSubjectId",
                principalTable: "ManagedStreamSubjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ManagedStreamSubjects_SelectedManagedStreamSubje~",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ManagedStreamSubjects");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SelectedManagedStreamSubjectId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SelectedManagedStreamSubjectId",
                table: "AspNetUsers");
        }
    }
}
