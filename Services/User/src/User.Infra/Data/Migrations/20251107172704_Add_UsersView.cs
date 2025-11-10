using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_UsersView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW vw_Users AS
                SELECT 
                    Id,
                    Username,
                    Email,
                    CreatedAt,
                    UpdatedAt
                FROM 
                    Users
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_Users");
        }
    }
}
