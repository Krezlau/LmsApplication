using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LmsApplication.CourseModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseEditionSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseEditionSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowAllToPost = table.Column<bool>(type: "bit", nullable: false),
                    CourseEditionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEditionSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseEditionSettings_CourseEditions_CourseEditionId",
                        column: x => x.CourseEditionId,
                        principalTable: "CourseEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseEditionSettings_CourseEditionId",
                table: "CourseEditionSettings",
                column: "CourseEditionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseEditionSettings");
        }
    }
}
