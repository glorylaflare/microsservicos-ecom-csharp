using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_MercadoPagoPreferenceView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR ALTER VIEW vw_Payments AS
            SELECT
                Id,
                OrderId,
                Amount,
                Status,
                CheckoutUrl,
                MercadoPagoPreference,
                ExpirationDate,
                CreatedAt,
                UpdatedAt
            FROM Payments
            ");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_Payments");
        }
    }
}
