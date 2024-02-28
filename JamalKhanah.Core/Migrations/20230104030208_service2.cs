using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamalKhanah.Core.Migrations
{
    /// <inheritdoc />
    public partial class Service2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Services",
                newName: "TitleEn");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "MainSections",
                newName: "TitleEn");

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                table: "MainSections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TitleAr",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TitleAr",
                table: "MainSections");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "Services",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "MainSections",
                newName: "Title");
        }
    }
}
