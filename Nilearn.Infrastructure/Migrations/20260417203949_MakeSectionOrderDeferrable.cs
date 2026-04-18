using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nilearn.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeSectionOrderDeferrable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DROP INDEX IF EXISTS ""IX_Sections_CourseId_Order"";
    ");

            migrationBuilder.Sql(@"
        ALTER TABLE ""Sections""
        DROP CONSTRAINT IF EXISTS ""IX_Sections_CourseId_Order"";
    ");

            migrationBuilder.Sql(@"
        ALTER TABLE ""Sections""
        ADD CONSTRAINT ""IX_Sections_CourseId_Order""
        UNIQUE (""CourseId"", ""Order"")
        DEFERRABLE INITIALLY DEFERRED;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Sections""
                DROP CONSTRAINT IF EXISTS ""IX_Sections_CourseId_Order"";");

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX ""IX_Sections_CourseId_Order""
                ON ""Sections"" (""CourseId"", ""Order"");");
        }
    }
}
