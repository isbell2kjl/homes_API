using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace blog_API.Migrations
{
    /// <inheritdoc />
    public partial class UserComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Comments_UsrId_fk",
                table: "Comments",
                column: "UsrId_fk");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UsrId_fk",
                table: "Comments",
                column: "UsrId_fk",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UsrId_fk",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UsrId_fk",
                table: "Comments");
        }
    }
}
