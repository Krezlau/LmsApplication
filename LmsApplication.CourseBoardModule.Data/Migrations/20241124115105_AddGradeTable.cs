using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LmsApplication.CourseBoardModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGradeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GradesTableRowDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseEditionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowType = table.Column<int>(type: "int", nullable: false),
                    IsSummed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradesTableRowDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GradesTableRowValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RowDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowType = table.Column<int>(type: "int", nullable: false),
                    TeacherComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeacherId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    GradesTableRowBoolValue_Value = table.Column<bool>(type: "bit", nullable: true),
                    GradesTableRowNumberValue_Value = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradesTableRowValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradesTableRowValues_GradesTableRowDefinitions_RowDefinitionId",
                        column: x => x.RowDefinitionId,
                        principalTable: "GradesTableRowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GradesTableRowValues_RowDefinitionId",
                table: "GradesTableRowValues",
                column: "RowDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GradesTableRowValues");

            migrationBuilder.DropTable(
                name: "GradesTableRowDefinitions");
        }
    }
}
