using Microsoft.EntityFrameworkCore.Migrations;

namespace HypeStock.Migrations
{
    public partial class AddedLikeRatioColumnToBrands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Dislikes",
                table: "Brands",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Brands",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dislikes",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Brands");
        }
    }
}
