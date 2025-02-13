using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountHub.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class ipAddress_column_Account_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Accounts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Accounts");
        }
    }
}
