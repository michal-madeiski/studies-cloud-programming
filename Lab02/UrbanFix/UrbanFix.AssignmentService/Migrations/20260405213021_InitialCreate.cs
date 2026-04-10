using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UrbanFix.AssignmentService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "technical_teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technical_teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "task_assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_task_assignments_technical_teams_AssignedTeamId",
                        column: x => x.AssignedTeamId,
                        principalTable: "technical_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "technical_teams",
                columns: new[] { "Id", "IsAvailable", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), true, "Team_01" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), true, "Team_02" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), true, "Team_03" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), true, "Team_04" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), true, "Team_05" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), true, "Team_06" },
                    { new Guid("10000000-0000-0000-0000-000000000007"), true, "Team_07" },
                    { new Guid("10000000-0000-0000-0000-000000000008"), true, "Team_08" },
                    { new Guid("10000000-0000-0000-0000-000000000009"), true, "Team_09" },
                    { new Guid("10000000-0000-0000-0000-000000000010"), true, "Team_10" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_task_assignments_AssignedTeamId",
                table: "task_assignments",
                column: "AssignedTeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "task_assignments");

            migrationBuilder.DropTable(
                name: "technical_teams");
        }
    }
}
