using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Train.Migrations
{
    /// <inheritdoc />
    public partial class changeBatchTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_batches",
                table: "batches");

            migrationBuilder.RenameTable(
                name: "batches",
                newName: "Batches");

            migrationBuilder.RenameColumn(
                name: "StarTime",
                table: "Batches",
                newName: "StartTime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Batches",
                table: "Batches",
                column: "number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Batches",
                table: "Batches");

            migrationBuilder.RenameTable(
                name: "Batches",
                newName: "batches");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "batches",
                newName: "StarTime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_batches",
                table: "batches",
                column: "number");
        }
    }
}
