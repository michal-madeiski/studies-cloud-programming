using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrbanFix.ReportService.Migrations
{
    /// <inheritdoc />
    public partial class IntegrationWithS3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "reports",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "S3BucketName",
                table: "reports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "S3ObjectKey",
                table: "reports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "reports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "S3BucketName",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "S3ObjectKey",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "reports");
        }
    }
}
