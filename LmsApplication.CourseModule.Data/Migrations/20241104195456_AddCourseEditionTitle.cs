using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LmsApplication.CourseModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseEditionTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CourseEditions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "CourseEditions");
        }
    }
}
