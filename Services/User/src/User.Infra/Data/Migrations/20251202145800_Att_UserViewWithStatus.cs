using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Att_UserViewWithStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER VIEW vw_Users AS
                SELECT 
                    Id,
                    Username,
                    Email,
                    Status,
                    CreatedAt,
                    UpdatedAt
                FROM 
                    Users
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER VIEW vw_Users AS
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
    }
}
