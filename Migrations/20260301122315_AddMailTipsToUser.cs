using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Afrilancer.Migrations
{
    /// <inheritdoc />
    public partial class AddMailTipsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MailTips",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MailTips",
                table: "Users");
        }
    }
}
