using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace homes_API.Migrations
{
    /// <inheritdoc />
    public partial class Post_Archive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<sbyte>(
                name: "Visible",
                table: "Posts",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);

            migrationBuilder.AddColumn<sbyte>(
                name: "Archive",
                table: "Posts",
                type: "tinyint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archive",
                table: "Posts");

            migrationBuilder.AlterColumn<bool>(
                name: "Visible",
                table: "Posts",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(sbyte),
                oldType: "tinyint",
                oldNullable: true);
        }
    }
}
