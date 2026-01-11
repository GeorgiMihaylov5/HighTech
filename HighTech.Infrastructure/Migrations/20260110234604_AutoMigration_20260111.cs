using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HighTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AutoMigration_20260111 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryFields_Categories_CategoryId",
                table: "CategoryFields");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryFields_Fields_FieldId",
                table: "CategoryFields");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFieldValues_Fields_FieldID",
                table: "ProductFieldValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFieldValues_Products_ProductID",
                table: "ProductFieldValues");

            migrationBuilder.DropIndex(
                name: "IX_Fields_Name",
                table: "Fields");

            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                table: "Fields",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Fields_Name",
                table: "Fields",
                column: "Name",
                unique: true,
                filter: "[IsRemoved] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true,
                filter: "[IsRemoved] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryFields_Categories_CategoryId",
                table: "CategoryFields",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryFields_Fields_FieldId",
                table: "CategoryFields",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFieldValues_Fields_FieldID",
                table: "ProductFieldValues",
                column: "FieldID",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFieldValues_Products_ProductID",
                table: "ProductFieldValues",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryFields_Categories_CategoryId",
                table: "CategoryFields");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryFields_Fields_FieldId",
                table: "CategoryFields");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFieldValues_Fields_FieldID",
                table: "ProductFieldValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductFieldValues_Products_ProductID",
                table: "ProductFieldValues");

            migrationBuilder.DropIndex(
                name: "IX_Fields_Name",
                table: "Fields");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsRemoved",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "IsRemoved",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_Name",
                table: "Fields",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryFields_Categories_CategoryId",
                table: "CategoryFields",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryFields_Fields_FieldId",
                table: "CategoryFields",
                column: "FieldId",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFieldValues_Fields_FieldID",
                table: "ProductFieldValues",
                column: "FieldID",
                principalTable: "Fields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFieldValues_Products_ProductID",
                table: "ProductFieldValues",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
