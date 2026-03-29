using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_MercadoPagoPaymentIdView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR ALTER VIEW vw_Payments AS
            SELECT
                Id,
                Provider,
                EventId,
                Action,
                ApiVersion,
                PaymentId,
                DateCreated,
                LiveMode,
                Type,
                UserId,
                Status,
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