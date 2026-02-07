using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "Email", "FirstName", "LastName", "PasswordHash", "Phone", "ProfilePhotoUrl", "Role" },
                values: new object[] { -1L, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system@petminder.com", "System", "Account", "NO_LOGIN_SYSTEM_ACC", "000000000", null, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: -1L);
        }
    }
}
