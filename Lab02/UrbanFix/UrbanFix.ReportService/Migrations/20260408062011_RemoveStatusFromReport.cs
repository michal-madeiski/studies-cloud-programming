using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrbanFix.ReportService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStatusFromReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "reports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "reports",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
