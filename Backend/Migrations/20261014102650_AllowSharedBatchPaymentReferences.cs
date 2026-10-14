using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ZBooksApi.Models;

#nullable disable

namespace ZBooksApi.Migrations
{
    [DbContext(typeof(ZBooksDbContext))]
    [Migration("20261014102650_AllowSharedBatchPaymentReferences")]
    public partial class AllowSharedBatchPaymentReferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ__Payments__3A2B1927136D2CD9",
                table: "Payments");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentReference",
                table: "Payments",
                column: "PaymentReference");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentReference",
                table: "Payments");

            migrationBuilder.CreateIndex(
                name: "UQ__Payments__3A2B1927136D2CD9",
                table: "Payments",
                column: "PaymentReference",
                unique: true);
        }
    }
}
