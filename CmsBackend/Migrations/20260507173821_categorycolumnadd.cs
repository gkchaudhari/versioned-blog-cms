using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CmsBackend.Migrations
{
    /// <inheritdoc />
    public partial class categorycolumnadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Blogs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Blogs");
        }
    }
}
