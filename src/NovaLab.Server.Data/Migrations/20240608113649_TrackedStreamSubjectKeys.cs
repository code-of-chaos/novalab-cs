using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NovaLab.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrackedStreamSubjectKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackedStreamSubjectComponents_TrackedStreamSubjects_TrackedStreamSubjectId",
                table: "TrackedStreamSubjectComponents");

            migrationBuilder.DropIndex(
                name: "IX_TrackedStreamSubjectComponents_TrackedStreamSubjectId",
                table: "TrackedStreamSubjectComponents");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedStreamSubjectComponents_Id",
                table: "TrackedStreamSubjectComponents",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackedStreamSubjectComponents_TrackedStreamSubjects_Id",
                table: "TrackedStreamSubjectComponents",
                column: "Id",
                principalTable: "TrackedStreamSubjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackedStreamSubjectComponents_TrackedStreamSubjects_Id",
                table: "TrackedStreamSubjectComponents");

            migrationBuilder.DropIndex(
                name: "IX_TrackedStreamSubjectComponents_Id",
                table: "TrackedStreamSubjectComponents");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedStreamSubjectComponents_TrackedStreamSubjectId",
                table: "TrackedStreamSubjectComponents",
                column: "TrackedStreamSubjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackedStreamSubjectComponents_TrackedStreamSubjects_TrackedStreamSubjectId",
                table: "TrackedStreamSubjectComponents",
                column: "TrackedStreamSubjectId",
                principalTable: "TrackedStreamSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
