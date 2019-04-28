using Microsoft.EntityFrameworkCore.Migrations;

namespace BookStore.Migrations
{
    public partial class ChangeRelationInBookImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_BookId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ExtraLarge",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Large",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Meddium",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "SmallThumbnail",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "Thumbnail",
                table: "Images",
                newName: "ImageUrl");

            migrationBuilder.CreateIndex(
                name: "IX_Images_BookId",
                table: "Images",
                column: "BookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_BookId",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Images",
                newName: "Thumbnail");

            migrationBuilder.AddColumn<string>(
                name: "ExtraLarge",
                table: "Images",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Large",
                table: "Images",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meddium",
                table: "Images",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmallThumbnail",
                table: "Images",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Books",
                maxLength: 300,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_BookId",
                table: "Images",
                column: "BookId",
                unique: true);
        }
    }
}
