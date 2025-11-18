using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPoweredDefectManagementAssistant.Datalayer.Migrations
{
    /// <inheritdoc />
    public partial class CreateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Defects",
                columns: table => new
                {
                    TestCaseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Module = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExpectedResult = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActualResult = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GeneratedDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedStepsToReproduce = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuggestedSeverity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuggestedPriority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Embedding = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Defects", x => x.TestCaseId);
                });

            migrationBuilder.CreateTable(
                name: "TestSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestCaseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StepNumber = table.Column<int>(type: "int", nullable: false),
                    TestStep = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TestData = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ExpectedResult = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Screenshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExecutionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSteps_Defects_TestCaseId",
                        column: x => x.TestCaseId,
                        principalTable: "Defects",
                        principalColumn: "TestCaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestSteps_TestCaseId",
                table: "TestSteps",
                column: "TestCaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestSteps");

            migrationBuilder.DropTable(
                name: "Defects");
        }
    }
}
