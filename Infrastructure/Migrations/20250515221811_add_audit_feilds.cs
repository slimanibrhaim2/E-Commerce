using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_audit_feilds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_User",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_AttachmentType",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_BaseContent",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseContent_BaseContent",
                table: "BaseContent");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseContent_User",
                table: "BaseContent");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseItem_Category",
                table: "BaseItem");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseItem_User",
                table: "BaseItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_User",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_BaseItem",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Cart",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Category_Category",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_BaseContent",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_BaseItem",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMember_Conversation",
                table: "ConversationMember");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMember_User",
                table: "ConversationMember");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupon_BaseItem",
                table: "Coupon");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupon_DiscountType",
                table: "Coupon");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupon_User",
                table: "Coupon");

            migrationBuilder.DropForeignKey(
                name: "FK_Follower_User",
                table: "Follower");

            migrationBuilder.DropForeignKey(
                name: "FK_Follower_User1",
                table: "Follower");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_BaseContent",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Conversation",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_OrderActivity",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderActivity_OrderStatus",
                table: "OrderActivity");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_BaseItem",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Order",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Order",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentMethod",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentStatus",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_BaseItem",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductMedia_BaseItem",
                table: "ProductMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductMedia_MediaType",
                table: "ProductMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_BaseItem",
                table: "Service");

            migrationBuilder.DropTable(
                name: "AttachmentType");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "DiscountType");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "MediaType");

            migrationBuilder.DropTable(
                name: "OrderStatus");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropTable(
                name: "PaymentStatus");

            migrationBuilder.DropTable(
                name: "ProductFeatures");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "User");

            migrationBuilder.DropIndex(
                name: "PhoneNumberIndex",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_ProductMedia_MediaTypeId",
                table: "ProductMedia");

            migrationBuilder.DropIndex(
                name: "IX_Payment_MethodId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_PaymentStatusId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_OrderActivity_OrderStatusId",
                table: "OrderActivity");

            migrationBuilder.DropIndex(
                name: "IX_Notification_Id",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Follower",
                table: "Follower");

            migrationBuilder.DropIndex(
                name: "IX_Coupon_BaseItemId",
                table: "Coupon");

            migrationBuilder.DropIndex(
                name: "IX_Coupon_DiscountTypeId",
                table: "Coupon");

            migrationBuilder.DropIndex(
                name: "IX_BaseContent_ParentId",
                table: "BaseContent");

            migrationBuilder.DropIndex(
                name: "IX_Attachment_AttachmentTypeId",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "AddedAt",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "MediaTypeId",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "MethodId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentStatusId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "BecomeAt",
                table: "OrderActivity");

            migrationBuilder.DropColumn(
                name: "OrderStatusId",
                table: "OrderActivity");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "FollowedAt",
                table: "Follower");

            migrationBuilder.DropColumn(
                name: "BaseItemId",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "DiscountTypeId",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Conversation");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "AddedAt",
                table: "BaseItem");

            migrationBuilder.DropColumn(
                name: "ContentText",
                table: "BaseContent");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "BaseContent");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "BaseContent");

            migrationBuilder.DropColumn(
                name: "AttachmentTypeId",
                table: "Attachment");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Service",
                newName: "ServiceType");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "ProductMedia",
                newName: "MediaUrl");

            migrationBuilder.RenameColumn(
                name: "NotificationContenet",
                table: "Notification",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Message",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                table: "Coupon",
                newName: "DiscountAmount");

            migrationBuilder.RenameColumn(
                name: "CreateByUserId",
                table: "BaseContent",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BaseContent_CreateByUserId",
                table: "BaseContent",
                newName: "IX_BaseContent_UserId");

            migrationBuilder.RenameColumn(
                name: "ContentUrl",
                table: "Attachment",
                newName: "FileUrl");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePhoto",
                table: "User",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "User",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "User",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "User",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "User",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Service",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Service",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Service",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductMedia",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ProductMedia",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MediaType",
                table: "ProductMedia",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProductMedia",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Product",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Product",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "Product",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Product",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Payment",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Payment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Payment",
                type: "datetime2",
                nullable: true);

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

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Payment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OrderItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "OrderItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OrderItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OrderActivity",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "OrderActivity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderStatusDAOId",
                table: "OrderActivity",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "OrderActivity",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OrderActivity",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Order",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Order",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Notification",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Notification",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Notification",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Notification",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReadAt",
                table: "Message",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Message",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Message",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Message",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Follower",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Follower",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Follower",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Follower",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "BaseItemDAOId",
                table: "Coupon",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Coupon",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Coupon",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DiscountTypeDAOId",
                table: "Coupon",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "Coupon",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Coupon",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ConversationMember",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ConversationMember",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ConversationMember",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Conversation",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Conversation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Conversation",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Conversation",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Comment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Comment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Comment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Category",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Category",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Category",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Category",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "CartItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CartItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CartItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CartItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Cart",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Cart",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Cart",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BaseItem",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BaseItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "BaseItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BaseItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BaseContent",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "BaseContent",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BaseContent",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "BaseContent",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BaseContent",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "AttachmentTypeDAOId",
                table: "Attachment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Attachment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Attachment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "Attachment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Attachment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Address",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Address",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Address",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Address",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follower",
                table: "Follower",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationMember",
                table: "ConversationMember",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                table: "Address",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AttachmentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Favorite",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BaseItemDAOId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorite_BaseItem_BaseItemDAOId",
                        column: x => x.BaseItemDAOId,
                        principalTable: "BaseItem",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favorite_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Favorite_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductFeature",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFeature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductFeature_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandDAOProductDAO",
                columns: table => new
                {
                    BrandsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandDAOProductDAO", x => new { x.BrandsId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_BrandDAOProductDAO_Brands_BrandsId",
                        column: x => x.BrandsId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandDAOProductDAO_Product_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentMethodDAOId",
                table: "Payment",
                column: "PaymentMethodDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentStatusDAOId",
                table: "Payment",
                column: "PaymentStatusDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderActivity_OrderStatusDAOId",
                table: "OrderActivity",
                column: "OrderStatusDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderId",
                table: "Message",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Follower_FollowerId",
                table: "Follower",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_BaseItemDAOId",
                table: "Coupon",
                column: "BaseItemDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_DiscountTypeDAOId",
                table: "Coupon",
                column: "DiscountTypeDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_AttachmentTypeDAOId",
                table: "Attachment",
                column: "AttachmentTypeDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandDAOProductDAO_ProductsId",
                table: "BrandDAOProductDAO",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_BaseItemDAOId",
                table: "Favorite",
                column: "BaseItemDAOId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_ProductId",
                table: "Favorite",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_UserId",
                table: "Favorite",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeature_ProductId",
                table: "ProductFeature",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_User",
                table: "Address",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_AttachmentTypes_AttachmentTypeDAOId",
                table: "Attachment",
                column: "AttachmentTypeDAOId",
                principalTable: "AttachmentTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_BaseContent",
                table: "Attachment",
                column: "BaseContentId",
                principalTable: "BaseContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseContent_User",
                table: "BaseContent",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItem_Category",
                table: "BaseItem",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItem_User",
                table: "BaseItem",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_User",
                table: "Cart",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_BaseItem",
                table: "CartItem",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Cart",
                table: "CartItem",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Parent",
                table: "Category",
                column: "ParentId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_BaseContent",
                table: "Comment",
                column: "BaseContentId",
                principalTable: "BaseContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_BaseItem",
                table: "Comment",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMember_Conversation",
                table: "ConversationMember",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMember_User",
                table: "ConversationMember",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Coupon_BaseItem_BaseItemDAOId",
                table: "Coupon",
                column: "BaseItemDAOId",
                principalTable: "BaseItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupon_DiscountTypes_DiscountTypeDAOId",
                table: "Coupon",
                column: "DiscountTypeDAOId",
                principalTable: "DiscountTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupon_User",
                table: "Coupon",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Follower_Follower",
                table: "Follower",
                column: "FollowerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Follower_Following",
                table: "Follower",
                column: "FollowingId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_BaseContent",
                table: "Message",
                column: "BaseContentId",
                principalTable: "BaseContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Conversation",
                table: "Message",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Sender",
                table: "Message",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User",
                table: "Notification",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_OrderActivity",
                table: "Order",
                column: "OrderActivityId",
                principalTable: "OrderActivity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User",
                table: "Order",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderActivity_OrderStatuses_OrderStatusDAOId",
                table: "OrderActivity",
                column: "OrderStatusDAOId",
                principalTable: "OrderStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_BaseItem",
                table: "OrderItem",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Order",
                table: "OrderItem",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Order",
                table: "Payment",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Product_BaseItem",
                table: "Product",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMedia_BaseItem",
                table: "ProductMedia",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_BaseItem",
                table: "Service",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_User",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_AttachmentTypes_AttachmentTypeDAOId",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_BaseContent",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseContent_User",
                table: "BaseContent");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseItem_Category",
                table: "BaseItem");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseItem_User",
                table: "BaseItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_User",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_BaseItem",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Cart",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Category_Parent",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_BaseContent",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_BaseItem",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMember_Conversation",
                table: "ConversationMember");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMember_User",
                table: "ConversationMember");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupon_BaseItem_BaseItemDAOId",
                table: "Coupon");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupon_DiscountTypes_DiscountTypeDAOId",
                table: "Coupon");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupon_User",
                table: "Coupon");

            migrationBuilder.DropForeignKey(
                name: "FK_Follower_Follower",
                table: "Follower");

            migrationBuilder.DropForeignKey(
                name: "FK_Follower_Following",
                table: "Follower");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_BaseContent",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Conversation",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Sender",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_OrderActivity",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderActivity_OrderStatuses_OrderStatusDAOId",
                table: "OrderActivity");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_BaseItem",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Order",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Order",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentMethods_PaymentMethodDAOId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentStatuses_PaymentStatusDAOId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_BaseItem",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductMedia_BaseItem",
                table: "ProductMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_BaseItem",
                table: "Service");

            migrationBuilder.DropTable(
                name: "AttachmentTypes");

            migrationBuilder.DropTable(
                name: "BrandDAOProductDAO");

            migrationBuilder.DropTable(
                name: "DiscountTypes");

            migrationBuilder.DropTable(
                name: "Favorite");

            migrationBuilder.DropTable(
                name: "MediaTypes");

            migrationBuilder.DropTable(
                name: "OrderStatuses");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "PaymentStatuses");

            migrationBuilder.DropTable(
                name: "ProductFeature");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Payment_PaymentMethodDAOId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_PaymentStatusDAOId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_OrderActivity_OrderStatusDAOId",
                table: "OrderActivity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_UserId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Message_SenderId",
                table: "Message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Follower",
                table: "Follower");

            migrationBuilder.DropIndex(
                name: "IX_Follower_FollowerId",
                table: "Follower");

            migrationBuilder.DropIndex(
                name: "IX_Coupon_BaseItemDAOId",
                table: "Coupon");

            migrationBuilder.DropIndex(
                name: "IX_Coupon_DiscountTypeDAOId",
                table: "Coupon");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationMember",
                table: "ConversationMember");

            migrationBuilder.DropIndex(
                name: "IX_Attachment_AttachmentTypeDAOId",
                table: "Attachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductMedia");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
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
                name: "UpdatedAt",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrderActivity");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "OrderActivity");

            migrationBuilder.DropColumn(
                name: "OrderStatusDAOId",
                table: "OrderActivity");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrderActivity");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OrderActivity");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Follower");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Follower");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Follower");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Follower");

            migrationBuilder.DropColumn(
                name: "BaseItemDAOId",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "DiscountTypeDAOId",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ConversationMember");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ConversationMember");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ConversationMember");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Conversation");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Conversation");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Conversation");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Conversation");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BaseItem");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "BaseItem");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BaseItem");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BaseContent");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "BaseContent");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BaseContent");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "BaseContent");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BaseContent");

            migrationBuilder.DropColumn(
                name: "AttachmentTypeDAOId",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Address");

            migrationBuilder.RenameColumn(
                name: "ServiceType",
                table: "Service",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "MediaUrl",
                table: "ProductMedia",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Notification",
                newName: "NotificationContenet");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Message",
                newName: "ReceiverId");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "Coupon",
                newName: "DiscountValue");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BaseContent",
                newName: "CreateByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BaseContent_UserId",
                table: "BaseContent",
                newName: "IX_BaseContent_CreateByUserId");

            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "Attachment",
                newName: "ContentUrl");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePhoto",
                table: "User",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "User",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "User",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "User",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "User",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Duration",
                table: "Service",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedAt",
                table: "ProductMedia",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "MediaTypeId",
                table: "ProductMedia",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "Product",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Payment",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "MethodId",
                table: "Payment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentStatusId",
                table: "Payment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Payment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "BecomeAt",
                table: "OrderActivity",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "OrderStatusId",
                table: "OrderActivity",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Notification",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Notification",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "Notification",
                type: "datetime",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReadAt",
                table: "Message",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FollowedAt",
                table: "Follower",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "BaseItemId",
                table: "Coupon",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DiscountTypeId",
                table: "Coupon",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Coupon",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Coupon",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Coupon",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Conversation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "CartItem",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "CartItem",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BaseItem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedAt",
                table: "BaseItem",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ContentText",
                table: "BaseContent",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "BaseContent",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "BaseContent",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AttachmentTypeId",
                table: "Attachment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Address",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Follower",
                table: "Follower",
                columns: new[] { "FollowerId", "FollowingId" });

            migrationBuilder.CreateTable(
                name: "AttachmentType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brand_Brand",
                        column: x => x.ParentId,
                        principalTable: "Brand",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Brand_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DiscountType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorites_BaseItem",
                        column: x => x.BaseItemId,
                        principalTable: "BaseItem",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favorites_User",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MediaType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedId = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductFeatures_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "PhoneNumberIndex",
                table: "User",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedia_MediaTypeId",
                table: "ProductMedia",
                column: "MediaTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_MethodId",
                table: "Payment",
                column: "MethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentStatusId",
                table: "Payment",
                column: "PaymentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderActivity_OrderStatusId",
                table: "OrderActivity",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Id",
                table: "Notification",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_BaseItemId",
                table: "Coupon",
                column: "BaseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_DiscountTypeId",
                table: "Coupon",
                column: "DiscountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseContent_ParentId",
                table: "BaseContent",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_AttachmentTypeId",
                table: "Attachment",
                column: "AttachmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Brand_ParentId",
                table: "Brand",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Brand_ProductId",
                table: "Brand",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_BaseItemId",
                table: "Favorites",
                column: "BaseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserID",
                table: "Favorites",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatures_ProductId",
                table: "ProductFeatures",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_User",
                table: "Address",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_AttachmentType",
                table: "Attachment",
                column: "AttachmentTypeId",
                principalTable: "AttachmentType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_BaseContent",
                table: "Attachment",
                column: "BaseContentId",
                principalTable: "BaseContent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseContent_BaseContent",
                table: "BaseContent",
                column: "ParentId",
                principalTable: "BaseContent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseContent_User",
                table: "BaseContent",
                column: "CreateByUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItem_Category",
                table: "BaseItem",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseItem_User",
                table: "BaseItem",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_User",
                table: "Cart",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_BaseItem",
                table: "CartItem",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Cart",
                table: "CartItem",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Category",
                table: "Category",
                column: "ParentId",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_BaseContent",
                table: "Comment",
                column: "BaseContentId",
                principalTable: "BaseContent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_BaseItem",
                table: "Comment",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMember_Conversation",
                table: "ConversationMember",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMember_User",
                table: "ConversationMember",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupon_BaseItem",
                table: "Coupon",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupon_DiscountType",
                table: "Coupon",
                column: "DiscountTypeId",
                principalTable: "DiscountType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupon_User",
                table: "Coupon",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Follower_User",
                table: "Follower",
                column: "FollowerId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Follower_User1",
                table: "Follower",
                column: "FollowingId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_BaseContent",
                table: "Message",
                column: "BaseContentId",
                principalTable: "BaseContent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Conversation",
                table: "Message",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User",
                table: "Notification",
                column: "Id",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_OrderActivity",
                table: "Order",
                column: "OrderActivityId",
                principalTable: "OrderActivity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User",
                table: "Order",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderActivity_OrderStatus",
                table: "OrderActivity",
                column: "OrderStatusId",
                principalTable: "OrderStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_BaseItem",
                table: "OrderItem",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Order",
                table: "OrderItem",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Order",
                table: "Payment",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_PaymentMethod",
                table: "Payment",
                column: "MethodId",
                principalTable: "PaymentMethod",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_PaymentStatus",
                table: "Payment",
                column: "PaymentStatusId",
                principalTable: "PaymentStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_BaseItem",
                table: "Product",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMedia_BaseItem",
                table: "ProductMedia",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMedia_MediaType",
                table: "ProductMedia",
                column: "MediaTypeId",
                principalTable: "MediaType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_BaseItem",
                table: "Service",
                column: "BaseItemId",
                principalTable: "BaseItem",
                principalColumn: "Id");
        }
    }
}
