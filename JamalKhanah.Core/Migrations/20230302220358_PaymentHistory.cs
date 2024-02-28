using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JamalKhanah.Core.Migrations
{
    /// <inheritdoc />
    public partial class PaymentHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentUrlIdentifier",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Refunded = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Refundedat = table.Column<DateTime>(name: "Refunded_at", type: "datetime2", nullable: true),
                    Captured = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Capturedat = table.Column<DateTime>(name: "Captured_at", type: "datetime2", nullable: true),
                    Voidedat = table.Column<DateTime>(name: "Voided_at", type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Invoiceid = table.Column<string>(name: "Invoice_id", type: "nvarchar(max)", nullable: true),
                    Ip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amountformat = table.Column<string>(name: "Amount_format", type: "nvarchar(max)", nullable: true),
                    Feeformat = table.Column<string>(name: "Fee_format", type: "nvarchar(max)", nullable: true),
                    Refundedformat = table.Column<string>(name: "Refunded_format", type: "nvarchar(max)", nullable: true),
                    Capturedformat = table.Column<string>(name: "Captured_format", type: "nvarchar(max)", nullable: true),
                    Callbackurl = table.Column<string>(name: "Callback_url", type: "nvarchar(max)", nullable: true),
                    Createdat = table.Column<DateTime>(name: "Created_at", type: "datetime2", nullable: true),
                    Updatedat = table.Column<DateTime>(name: "Updated_at", type: "datetime2", nullable: true),
                    SourceType = table.Column<string>(name: "Source_Type", type: "nvarchar(max)", nullable: true),
                    SourceCompany = table.Column<string>(name: "Source_Company", type: "nvarchar(max)", nullable: true),
                    SourceName = table.Column<string>(name: "Source_Name", type: "nvarchar(max)", nullable: true),
                    SourceNumber = table.Column<string>(name: "Source_Number", type: "nvarchar(max)", nullable: true),
                    SourceGatewayid = table.Column<string>(name: "Source_Gateway_id", type: "nvarchar(max)", nullable: true),
                    SourceReferencenumber = table.Column<string>(name: "Source_Reference_number", type: "nvarchar(max)", nullable: true),
                    SourceToken = table.Column<string>(name: "Source_Token", type: "nvarchar(max)", nullable: true),
                    SourceMessage = table.Column<string>(name: "Source_Message", type: "nvarchar(max)", nullable: true),
                    SourceTransactionurl = table.Column<string>(name: "Source_Transaction_url", type: "nvarchar(max)", nullable: true),
                    TransactionNumber = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    PaymentUrlIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentHistories_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_OrderId",
                table: "PaymentHistories",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "PaymentUrlIdentifier",
                table: "Orders");
        }
    }
}
