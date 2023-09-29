using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSheetEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cells_sheets_sheet_id",
                schema: "public",
                table: "cells");

            migrationBuilder.DropTable(
                name: "sheets",
                schema: "public");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cells",
                schema: "public",
                table: "cells");

            migrationBuilder.DropIndex(
                name: "IX_cells_sheet_id",
                schema: "public",
                table: "cells");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "public",
                table: "cells",
                newName: "cell_id");

            migrationBuilder.AlterColumn<string>(
                name: "sheet_id",
                schema: "public",
                table: "cells",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_cells",
                schema: "public",
                table: "cells",
                columns: new[] { "sheet_id", "cell_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_cells",
                schema: "public",
                table: "cells");

            migrationBuilder.RenameColumn(
                name: "cell_id",
                schema: "public",
                table: "cells",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "sheet_id",
                schema: "public",
                table: "cells",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cells",
                schema: "public",
                table: "cells",
                column: "id");

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

            migrationBuilder.CreateIndex(
                name: "IX_cells_sheet_id",
                schema: "public",
                table: "cells",
                column: "sheet_id");

            migrationBuilder.AddForeignKey(
                name: "FK_cells_sheets_sheet_id",
                schema: "public",
                table: "cells",
                column: "sheet_id",
                principalSchema: "public",
                principalTable: "sheets",
                principalColumn: "id");
        }
    }
}
