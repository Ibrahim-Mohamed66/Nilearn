using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nilearn.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixWalletTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstructorWallet_Instructors_InstructorId",
                table: "InstructorWallet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlatformWallet",
                table: "PlatformWallet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstructorWallet",
                table: "InstructorWallet");

            migrationBuilder.RenameTable(
                name: "PlatformWallet",
                newName: "PlatformWallets");

            migrationBuilder.RenameTable(
                name: "InstructorWallet",
                newName: "InstructorWallets");

            migrationBuilder.RenameIndex(
                name: "IX_InstructorWallet_InstructorId",
                table: "InstructorWallets",
                newName: "IX_InstructorWallets_InstructorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlatformWallets",
                table: "PlatformWallets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstructorWallets",
                table: "InstructorWallets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorWallets_Instructors_InstructorId",
                table: "InstructorWallets",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstructorWallets_Instructors_InstructorId",
                table: "InstructorWallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlatformWallets",
                table: "PlatformWallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstructorWallets",
                table: "InstructorWallets");

            migrationBuilder.RenameTable(
                name: "PlatformWallets",
                newName: "PlatformWallet");

            migrationBuilder.RenameTable(
                name: "InstructorWallets",
                newName: "InstructorWallet");

            migrationBuilder.RenameIndex(
                name: "IX_InstructorWallets_InstructorId",
                table: "InstructorWallet",
                newName: "IX_InstructorWallet_InstructorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlatformWallet",
                table: "PlatformWallet",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstructorWallet",
                table: "InstructorWallet",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorWallet_Instructors_InstructorId",
                table: "InstructorWallet",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
