using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraSheetId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cell_dependencies_cells_depended_by_sheet_id_depended_by_ce~",
                schema: "public",
                table: "cell_dependencies");

            migrationBuilder.DropForeignKey(
                name: "FK_cell_dependencies_cells_depended_sheet_id_depended_cell_id",
                schema: "public",
                table: "cell_dependencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cell_dependencies",
                schema: "public",
                table: "cell_dependencies");

            migrationBuilder.DropColumn(
                name: "depended_sheet_id",
                schema: "public",
                table: "cell_dependencies");

            migrationBuilder.RenameColumn(
                name: "depended_by_sheet_id",
                schema: "public",
                table: "cell_dependencies",
                newName: "sheet_id");

            migrationBuilder.RenameIndex(
                name: "IX_cell_dependencies_depended_by_sheet_id_depended_by_cell_id",
                schema: "public",
                table: "cell_dependencies",
                newName: "IX_cell_dependencies_sheet_id_depended_by_cell_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cell_dependencies",
                schema: "public",
                table: "cell_dependencies",
                columns: new[] { "sheet_id", "depended_cell_id", "depended_by_cell_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_cell_dependencies_cells_sheet_id_depended_by_cell_id",
                schema: "public",
                table: "cell_dependencies",
                columns: new[] { "sheet_id", "depended_by_cell_id" },
                principalSchema: "public",
                principalTable: "cells",
                principalColumns: new[] { "sheet_id", "cell_id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cell_dependencies_cells_sheet_id_depended_cell_id",
                schema: "public",
                table: "cell_dependencies",
                columns: new[] { "sheet_id", "depended_cell_id" },
                principalSchema: "public",
                principalTable: "cells",
                principalColumns: new[] { "sheet_id", "cell_id" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cell_dependencies_cells_sheet_id_depended_by_cell_id",
                schema: "public",
                table: "cell_dependencies");

            migrationBuilder.DropForeignKey(
                name: "FK_cell_dependencies_cells_sheet_id_depended_cell_id",
                schema: "public",
                table: "cell_dependencies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cell_dependencies",
                schema: "public",
                table: "cell_dependencies");

            migrationBuilder.RenameColumn(
                name: "sheet_id",
                schema: "public",
                table: "cell_dependencies",
                newName: "depended_by_sheet_id");

            migrationBuilder.RenameIndex(
                name: "IX_cell_dependencies_sheet_id_depended_by_cell_id",
                schema: "public",
                table: "cell_dependencies",
                newName: "IX_cell_dependencies_depended_by_sheet_id_depended_by_cell_id");

            migrationBuilder.AddColumn<string>(
                name: "depended_sheet_id",
                schema: "public",
                table: "cell_dependencies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cell_dependencies",
                schema: "public",
                table: "cell_dependencies",
                columns: new[] { "depended_sheet_id", "depended_cell_id", "depended_by_sheet_id", "depended_by_cell_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_cell_dependencies_cells_depended_by_sheet_id_depended_by_ce~",
                schema: "public",
                table: "cell_dependencies",
                columns: new[] { "depended_by_sheet_id", "depended_by_cell_id" },
                principalSchema: "public",
                principalTable: "cells",
                principalColumns: new[] { "sheet_id", "cell_id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cell_dependencies_cells_depended_sheet_id_depended_cell_id",
                schema: "public",
                table: "cell_dependencies",
                columns: new[] { "depended_sheet_id", "depended_cell_id" },
                principalSchema: "public",
                principalTable: "cells",
                principalColumns: new[] { "sheet_id", "cell_id" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
