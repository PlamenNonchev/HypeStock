using Microsoft.EntityFrameworkCore.Migrations;

namespace HypeStock.Migrations
{
    public partial class AddedUserProductsLikestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dislikes",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Brands");

            migrationBuilder.CreateTable(
                name: "UserProductLikes",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProductLikes", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_UserProductLikes_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProductLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProductLikes_ProductId",
                table: "UserProductLikes",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProductLikes");

            migrationBuilder.AddColumn<int>(
                name: "Dislikes",
                table: "Brands",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Brands",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
