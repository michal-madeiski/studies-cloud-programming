using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrbanFix.ReportService.Migrations
{
    /// <inheritdoc />
    public partial class GeoCoords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "reports");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "reports",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "reports",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "reports");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "reports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
