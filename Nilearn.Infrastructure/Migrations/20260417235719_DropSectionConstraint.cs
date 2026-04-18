using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nilearn.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropSectionConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
                ALTER TABLE ""Sections""
                DROP CONSTRAINT IF EXISTS ""IX_Sections_CourseId_Order"";
            ");
            

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId",
                table: "Sections",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sections_CourseId",
                table: "Sections");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId_Order",
                table: "Sections",
                columns: new[] { "CourseId", "Order" },
                unique: true);
        }
    }
}
