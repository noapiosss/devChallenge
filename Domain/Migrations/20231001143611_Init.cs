using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "cells",
                schema: "public",
                columns: table => new
                {
                    sheet_id = table.Column<string>(type: "text", nullable: false),
                    cell_id = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true),
                    is_expression = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cells", x => new { x.sheet_id, x.cell_id });
                });

            migrationBuilder.CreateTable(
                name: "cell_dependencies",
                schema: "public",
                columns: table => new
                {
                    depended_sheet_id = table.Column<string>(type: "text", nullable: false),
                    depended_cell_id = table.Column<string>(type: "text", nullable: false),
                    depended_by_sheet_id = table.Column<string>(type: "text", nullable: false),
                    depended_by_cell_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cell_dependencies", x => new { x.depended_sheet_id, x.depended_cell_id, x.depended_by_sheet_id, x.depended_by_cell_id });
                    table.ForeignKey(
                        name: "FK_cell_dependencies_cells_depended_by_sheet_id_depended_by_ce~",
                        columns: x => new { x.depended_by_sheet_id, x.depended_by_cell_id },
                        principalSchema: "public",
                        principalTable: "cells",
                        principalColumns: new[] { "sheet_id", "cell_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cell_dependencies_cells_depended_sheet_id_depended_cell_id",
                        columns: x => new { x.depended_sheet_id, x.depended_cell_id },
                        principalSchema: "public",
                        principalTable: "cells",
                        principalColumns: new[] { "sheet_id", "cell_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cell_dependencies_depended_by_sheet_id_depended_by_cell_id",
                schema: "public",
                table: "cell_dependencies",
                columns: new[] { "depended_by_sheet_id", "depended_by_cell_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cell_dependencies",
                schema: "public");

            migrationBuilder.DropTable(
                name: "cells",
                schema: "public");
        }
    }
}
