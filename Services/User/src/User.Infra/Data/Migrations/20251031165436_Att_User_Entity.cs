using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Att_User_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "Auth0UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Auth0UserId",
                table: "Users",
                column: "Auth0UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Auth0UserId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Auth0UserId",
                table: "Users",
                newName: "Password");
        }
    }
}
