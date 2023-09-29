using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "sheets",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sheets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cells",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    sheet_id = table.Column<string>(type: "text", nullable: true),
                    value = table.Column<string>(type: "text", nullable: true),
                    is_expression = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cells", x => x.id);
                    table.ForeignKey(
                        name: "FK_cells_sheets_sheet_id",
                        column: x => x.sheet_id,
                        principalSchema: "public",
                        principalTable: "sheets",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_cells_sheet_id",
                schema: "public",
                table: "cells",
                column: "sheet_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cells",
                schema: "public");

            migrationBuilder.DropTable(
                name: "sheets",
                schema: "public");
        }
    }
}
