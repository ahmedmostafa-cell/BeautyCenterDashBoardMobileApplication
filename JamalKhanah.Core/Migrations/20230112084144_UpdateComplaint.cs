using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamalKhanah.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateComplaint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Complaints");
        }
    }
}
