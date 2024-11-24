using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LmsApplication.CourseBoardModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeValueColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "GradesTableRowValues",
                newName: "TextValue");

            migrationBuilder.RenameColumn(
                name: "GradesTableRowNumberValue_Value",
                table: "GradesTableRowValues",
                newName: "NumberValue");

            migrationBuilder.RenameColumn(
                name: "GradesTableRowBoolValue_Value",
                table: "GradesTableRowValues",
                newName: "BoolValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TextValue",
                table: "GradesTableRowValues",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "NumberValue",
                table: "GradesTableRowValues",
                newName: "GradesTableRowNumberValue_Value");

            migrationBuilder.RenameColumn(
                name: "BoolValue",
                table: "GradesTableRowValues",
                newName: "GradesTableRowBoolValue_Value");
        }
    }
}
