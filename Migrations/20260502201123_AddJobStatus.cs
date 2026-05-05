using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrilancer.Migrations
{
    /// <inheritdoc />
    public partial class AddJobStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Jobs",
                type: "text",
                nullable: false,
                defaultValue: "open");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Jobs");
        }
    }
}
