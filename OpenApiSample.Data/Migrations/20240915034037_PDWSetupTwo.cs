using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenApiSample.Data.Migrations
{
    /// <inheritdoc />
    public partial class PDWSetupTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Pdw_PdwId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pdw",
                table: "Pdw");

            migrationBuilder.RenameTable(
                name: "Pdw",
                newName: "Pdws");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pdws",
                table: "Pdws",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Pdws_PdwId",
                table: "Projects",
                column: "PdwId",
                principalTable: "Pdws",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Pdws_PdwId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pdws",
                table: "Pdws");

            migrationBuilder.RenameTable(
                name: "Pdws",
                newName: "Pdw");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pdw",
                table: "Pdw",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Pdw_PdwId",
                table: "Projects",
                column: "PdwId",
                principalTable: "Pdw",
                principalColumn: "Id");
        }
    }
}
