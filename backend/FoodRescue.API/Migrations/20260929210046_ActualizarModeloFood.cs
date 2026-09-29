using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodRescue.API.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarModeloFood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Foods",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Foods",
                newName: "Responsible");

            migrationBuilder.AddColumn<string>(
                name: "FoodType",
                table: "Foods",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Product",
                table: "Foods",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoodType",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "Product",
                table: "Foods");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Foods",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Responsible",
                table: "Foods",
                newName: "Description");
        }
    }
}
