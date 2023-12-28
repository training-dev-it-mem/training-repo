using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Train.Migrations
{
    /// <inheritdoc />
    public partial class AddIdToBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "number",
                table: "Batches",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Batches",
                newName: "number");
        }
    }
}
