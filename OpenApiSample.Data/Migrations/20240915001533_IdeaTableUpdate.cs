using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenApiSample.Data.Migrations
{
    /// <inheritdoc />
    public partial class IdeaTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Ideas");

            migrationBuilder.AddColumn<bool>(
                name: "ClientApproved",
                table: "Ideas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TechLeadApproved",
                table: "Ideas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientApproved",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "TechLeadApproved",
                table: "Ideas");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Ideas",
                type: "int",
                nullable: true);
        }
    }
}
