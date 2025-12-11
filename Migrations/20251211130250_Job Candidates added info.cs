using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class JobCandidatesaddedinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "HumanResources",
                table: "JobCandidate",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "HumanResources",
                table: "JobCandidate",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "HumanResources",
                table: "JobCandidate",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                schema: "HumanResources",
                table: "JobCandidate",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                schema: "HumanResources",
                table: "JobCandidate",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                schema: "HumanResources",
                table: "JobCandidate",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "HumanResources",
                table: "JobCandidate");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "HumanResources",
                table: "JobCandidate");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "HumanResources",
                table: "JobCandidate");

            migrationBuilder.DropColumn(
                name: "Message",
                schema: "HumanResources",
                table: "JobCandidate");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                schema: "HumanResources",
                table: "JobCandidate");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "HumanResources",
                table: "JobCandidate");
        }
    }
}
