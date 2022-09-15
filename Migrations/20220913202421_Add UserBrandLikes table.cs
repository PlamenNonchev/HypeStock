using Microsoft.EntityFrameworkCore.Migrations;

namespace HypeStock.Migrations
{
    public partial class AddUserBrandLikestable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBrandLikes",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    BrandId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBrandLikes", x => new { x.UserId, x.BrandId });
                    table.ForeignKey(
                        name: "FK_UserBrandLikes_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBrandLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBrandLikes_BrandId",
                table: "UserBrandLikes",
                column: "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBrandLikes");
        }
    }
}
