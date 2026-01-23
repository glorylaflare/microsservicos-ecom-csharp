using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Order.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_OrdersView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW vw_Orders AS
            SELECT
                Id,
                Status,
                TotalAmount,
                CreatedAt,
                UpdatedAt
            FROM Orders
            ");
            migrationBuilder.Sql(@"
            CREATE VIEW vw_OrderItems AS
            SELECT
                OrderId,
                ProductId,
                Quantity
            FROM OrderItem
            ");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_OrderItems");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_Orders");
        }
    }
}