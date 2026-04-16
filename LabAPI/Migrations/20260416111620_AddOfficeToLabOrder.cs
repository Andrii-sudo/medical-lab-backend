using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddOfficeToLabOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "office_id",
                table: "lab_order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_lab_order_office_id",
                table: "lab_order",
                column: "office_id");

            migrationBuilder.AddForeignKey(
                name: "lab_order_office_fkey",
                table: "lab_order",
                column: "office_id",
                principalTable: "office",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "lab_order_office_fkey",
                table: "lab_order");

            migrationBuilder.DropIndex(
                name: "IX_lab_order_office_id",
                table: "lab_order");

            migrationBuilder.DropColumn(
                name: "office_id",
                table: "lab_order");
        }
    }
}
