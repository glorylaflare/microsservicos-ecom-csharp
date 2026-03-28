using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_MercadoPagoPreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MercadoPagoPreference",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MercadoPagoPreference",
                table: "Payments");
        }
    }
}
