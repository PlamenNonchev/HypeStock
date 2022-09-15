using Microsoft.EntityFrameworkCore.Migrations;

namespace HypeStock.Migrations
{
    public partial class RemoveLikeAndDislikesFromProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dislikes",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Dislikes",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
