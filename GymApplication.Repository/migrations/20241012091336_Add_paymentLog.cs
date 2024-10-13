using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymApplication.Repository.migrations
{
    /// <inheritdoc />
    public partial class Add_paymentLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_PaymentLogs_PaymentId",
                table: "UserSubscriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_PaymentLogs_PaymentId",
                table: "UserSubscriptions",
                column: "PaymentId",
                principalTable: "PaymentLogs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_PaymentLogs_PaymentId",
                table: "UserSubscriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_PaymentLogs_PaymentId",
                table: "UserSubscriptions",
                column: "PaymentId",
                principalTable: "PaymentLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
