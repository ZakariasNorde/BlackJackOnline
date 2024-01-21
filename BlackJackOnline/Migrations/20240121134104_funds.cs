using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlackJackOnline.Migrations
{
    /// <inheritdoc />
    public partial class funds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Funds",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Funds",
                table: "AspNetUsers");
        }
    }
}
