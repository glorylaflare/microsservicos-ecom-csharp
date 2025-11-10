using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_ProductsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW vw_Products AS
                SELECT 
                    Id,
                    Name,
                    Description,
                    Price,
                    StockQuantity,
                    CreatedAt,
                    UpdatedAt
                FROM 
                    Products
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_Products");
        }
    }
}
