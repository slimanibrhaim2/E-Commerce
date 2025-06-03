using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentMethods_PaymentMethodDAOId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentStatuses_PaymentStatusDAOId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_PaymentMethodDAOId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_PaymentStatusDAOId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentMethodDAOId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentStatusDAOId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrderActivity");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMethodId",
                table: "Payment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "Payment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "OrderActivity",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentMethodId",
                table: "Payment",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_StatusId",
                table: "Payment",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_PaymentMethod",
                table: "Payment",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_PaymentStatus",
                table: "Payment",
                column: "StatusId",
                principalTable: "PaymentStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentMethod",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentStatus",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_PaymentMethodId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_StatusId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "OrderActivity");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMethodDAOId",
                table: "Payment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentStatusDAOId",
                table: "Payment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "OrderActivity",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentMethodDAOId",
                table: "Payment",
                column: "PaymentMethodDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentStatusDAOId",
                table: "Payment",
                column: "PaymentStatusDAOId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_PaymentMethods_PaymentMethodDAOId",
                table: "Payment",
                column: "PaymentMethodDAOId",
                principalTable: "PaymentMethods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_PaymentStatuses_PaymentStatusDAOId",
                table: "Payment",
                column: "PaymentStatusDAOId",
                principalTable: "PaymentStatuses",
                principalColumn: "Id");
        }
    }
}
