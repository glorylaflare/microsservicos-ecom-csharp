using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_NewViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR ALTER VIEW vw_WebhookEvents AS
            SELECT
                we.Id,
                we.Provider,
                we.EventId,
                we.Status,
                wp.Action,
                wp.ApiVersion,
                wp.PaymentId,
                wp.DateCreated,
                wp.LiveMode,
                wp.Type,
                wp.UserId,
                we.CreatedAt,
                we.UpdatedAt
            FROM WebhookEvents we
            LEFT JOIN WebhookPayload wp
                ON wp.WebhookEventId = we.Id
            ");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_WebhookEvents");
        }
    }
}
