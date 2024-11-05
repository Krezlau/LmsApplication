using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LmsApplication.CourseModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseEditionRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParticipantEmail",
                table: "CourseEditionParticipants",
                newName: "ParticipantId");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationEndDateUtc",
                table: "CourseEditions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationStartDateUtc",
                table: "CourseEditions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationEndDateUtc",
                table: "CourseEditions");

            migrationBuilder.DropColumn(
                name: "RegistrationStartDateUtc",
                table: "CourseEditions");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "CourseEditionParticipants",
                newName: "ParticipantEmail");
        }
    }
}
