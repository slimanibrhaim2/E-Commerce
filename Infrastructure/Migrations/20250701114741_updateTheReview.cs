using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateTheReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_BaseItem",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Order",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_User",
                table: "Review");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Review",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_UserId_BaseItemId_Unique",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Review");

            migrationBuilder.RenameTable(
                name: "Review",
                newName: "Reviews");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reviews",
                newName: "ReviewerId");

            migrationBuilder.RenameColumn(
                name: "IsVerifiedPurchase",
                table: "Reviews",
                newName: "WillUseAgain");

            migrationBuilder.RenameColumn(
                name: "BaseItemId",
                table: "Reviews",
                newName: "ProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_OrderId",
                table: "Reviews",
                newName: "IX_Reviews_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_BaseItemId",
                table: "Reviews",
                newName: "IX_Reviews_ProviderId");

            migrationBuilder.AddColumn<Guid>(
                name: "BaseItemDAOId",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Communication",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ExperienceDescription",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ItemQuality",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NetPromoterScore",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderDAOId",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OverallSatisfaction",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Timeliness",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDAOId",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValueForMoney",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BaseItemDAOId",
                table: "Reviews",
                column: "BaseItemDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OrderDAOId",
                table: "Reviews",
                column: "OrderDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserDAOId",
                table: "Reviews",
                column: "UserDAOId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_BaseItem_BaseItemDAOId",
                table: "Reviews",
                column: "BaseItemDAOId",
                principalTable: "BaseItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Order_OrderDAOId",
                table: "Reviews",
                column: "OrderDAOId",
                principalTable: "Order",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Order_OrderId",
                table: "Reviews",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_User_ProviderId",
                table: "Reviews",
                column: "ProviderId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_User_ReviewerId",
                table: "Reviews",
                column: "ReviewerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_User_UserDAOId",
                table: "Reviews",
                column: "UserDAOId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_BaseItem_BaseItemDAOId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Order_OrderDAOId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Order_OrderId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_User_ProviderId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_User_ReviewerId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_User_UserDAOId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BaseItemDAOId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_OrderDAOId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserDAOId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "BaseItemDAOId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Communication",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ExperienceDescription",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ItemQuality",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "NetPromoterScore",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "OrderDAOId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "OverallSatisfaction",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Timeliness",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UserDAOId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ValueForMoney",
                table: "Reviews");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "Review");

            migrationBuilder.RenameColumn(
                name: "WillUseAgain",
                table: "Review",
                newName: "IsVerifiedPurchase");

            migrationBuilder.RenameColumn(
                name: "ReviewerId",
                table: "Review",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "Review",
                newName: "BaseItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_ProviderId",
                table: "Review",
                newName: "IX_Review_BaseItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_OrderId",
                table: "Review",
                newName: "IX_Review_OrderId");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Review",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Review",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Review",
                table: "Review",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserId_BaseItemId_Unique",
                table: "Review",
                columns: new[] { "UserId", "BaseItemId" },
                unique: true,
                filter: "[DeletedAt] IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_BaseItem",
                table: "Review",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Order",
                table: "Review",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_User",
                table: "Review",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
