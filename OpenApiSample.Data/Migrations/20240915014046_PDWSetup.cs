using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenApiSample.Data.Migrations
{
    /// <inheritdoc />
    public partial class PDWSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PdwId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Pdw",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JsonData = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pdw", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PdwId",
                table: "Projects",
                column: "PdwId",
                unique: true,
                filter: "[PdwId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Pdw_PdwId",
                table: "Projects",
                column: "PdwId",
                principalTable: "Pdw",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Pdw_PdwId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "Pdw");

            migrationBuilder.DropIndex(
                name: "IX_Projects_PdwId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PdwId",
                table: "Projects");
        }
    }
}
