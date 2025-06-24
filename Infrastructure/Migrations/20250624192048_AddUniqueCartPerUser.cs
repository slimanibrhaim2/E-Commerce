using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueCartPerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_BaseItem_BaseItemDAOId",
                table: "Favorite");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_Product",
                table: "Favorite");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.DropIndex(
                name: "IX_Favorite_BaseItemDAOId",
                table: "Favorite");

            migrationBuilder.DropIndex(
                name: "IX_Cart_UserId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "BaseItemDAOId",
                table: "Favorite");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Favorite",
                newName: "BaseItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Favorite_ProductId",
                table: "Favorite",
                newName: "IX_Favorite_BaseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserId_Active",
                table: "Cart",
                column: "UserId",
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_BaseItem",
                table: "Favorite",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_BaseItem",
                table: "Favorite");

            migrationBuilder.DropIndex(
                name: "IX_Cart_UserId_Active",
                table: "Cart");

            migrationBuilder.RenameColumn(
                name: "BaseItemId",
                table: "Favorite",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Favorite_BaseItemId",
                table: "Favorite",
                newName: "IX_Favorite_ProductId");

            migrationBuilder.AddColumn<Guid>(
                name: "BaseItemDAOId",
                table: "Favorite",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseItemDAOId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DiscountAmount = table.Column<double>(type: "float", nullable: false),
                    DiscountTypeDAOId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coupon_BaseItem_BaseItemDAOId",
                        column: x => x.BaseItemDAOId,
                        principalTable: "BaseItem",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Coupon_DiscountTypes_DiscountTypeDAOId",
                        column: x => x.DiscountTypeDAOId,
                        principalTable: "DiscountTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Coupon_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_BaseItemDAOId",
                table: "Favorite",
                column: "BaseItemDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserId",
                table: "Cart",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_BaseItemDAOId",
                table: "Coupon",
                column: "BaseItemDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_DiscountTypeDAOId",
                table: "Coupon",
                column: "DiscountTypeDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_UserId",
                table: "Coupon",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_BaseItem_BaseItemDAOId",
                table: "Favorite",
                column: "BaseItemDAOId",
                principalTable: "BaseItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_Product",
                table: "Favorite",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
