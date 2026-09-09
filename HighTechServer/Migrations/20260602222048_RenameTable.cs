using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HighTechServer.Migrations
{
    /// <inheritdoc />
    public partial class RenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductsCategories");

            migrationBuilder.CreateTable(
                name: "ProductFieldValues",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    CategoryFieldId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFieldValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductFieldValues_CategoryFields_CategoryFieldId",
                        column: x => x.CategoryFieldId,
                        principalTable: "CategoryFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductFieldValues_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductFieldValues_CategoryFieldId",
                table: "ProductFieldValues",
                column: "CategoryFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFieldValues_ProductId",
                table: "ProductFieldValues",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductFieldValues");

            migrationBuilder.CreateTable(
                name: "ProductsCategories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    CategoryFieldId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsCategories_CategoryFields_CategoryFieldId",
                        column: x => x.CategoryFieldId,
                        principalTable: "CategoryFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductsCategories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductsCategories_CategoryFieldId",
                table: "ProductsCategories",
                column: "CategoryFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsCategories_ProductId",
                table: "ProductsCategories",
                column: "ProductId");
        }
    }
}
