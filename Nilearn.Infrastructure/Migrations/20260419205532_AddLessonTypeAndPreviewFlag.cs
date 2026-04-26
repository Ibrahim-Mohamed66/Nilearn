using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nilearn.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonTypeAndPreviewFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "SecureResourceUrl",
                table: "Lessons");

            migrationBuilder.AddColumn<bool>(
                name: "IsPreview",
                table: "Lessons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LessonType",
                table: "Lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPreview",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "LessonType",
                table: "Lessons");

            migrationBuilder.AddColumn<int>(
                name: "ResourceType",
                table: "Lessons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecureResourceUrl",
                table: "Lessons",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
