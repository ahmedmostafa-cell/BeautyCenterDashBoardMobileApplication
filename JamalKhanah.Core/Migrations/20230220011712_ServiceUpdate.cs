using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamalKhanah.Core.Migrations
{
    /// <inheritdoc />
    public partial class ServiceUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_UserId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Services",
                newName: "ProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_UserId",
                table: "Services",
                newName: "IX_Services_ProviderId");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "EvaluationServices",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "EvaluationProviders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Users_ProviderId",
                table: "Services",
                column: "ProviderId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_ProviderId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "EvaluationServices");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "EvaluationProviders");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "Services",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ProviderId",
                table: "Services",
                newName: "IX_Services_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Users_UserId",
                table: "Services",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
