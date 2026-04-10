using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrbanFix.AssignmentService.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmitterEmailToTaskAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubmitterEmail",
                table: "task_assignments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmitterEmail",
                table: "task_assignments");
        }
    }
}
