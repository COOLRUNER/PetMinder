using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:address_type", "primary,secondary,other")
                .Annotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled,completed")
                .Annotation("Npgsql:Enum:cancellation_policy_name", "standard,late")
                .Annotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .Annotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .Annotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,change_requested,change_accepted,change_rejected,system_alert")
                .Annotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .Annotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment,verification_reward")
                .Annotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .Annotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .Annotation("Npgsql:Enum:verification_step", "email_verification,phone_verification,profile_photo_upload,identity_verification,home_verification")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled,completed")
                .OldAnnotation("Npgsql:Enum:cancellation_policy_name", "standard,late")
                .OldAnnotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,change_requested,change_accepted,change_rejected,system_alert")
                .OldAnnotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .OldAnnotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment,verification_reward")
                .OldAnnotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .OldAnnotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .OldAnnotation("Npgsql:Enum:verification_step", "email_verification,phone_verification,profile_photo_upload,identity_verification,home_verification");

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Street = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    City = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                columns: table => new
                {
                    UserAddressId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AddressId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.UserAddressId);
                    table.ForeignKey(
                        name: "FK_UserAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAddresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_AddressId",
                table: "UserAddresses",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId_AddressId",
                table: "UserAddresses",
                columns: new[] { "UserId", "AddressId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled,completed")
                .Annotation("Npgsql:Enum:cancellation_policy_name", "standard,late")
                .Annotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .Annotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .Annotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,change_requested,change_accepted,change_rejected,system_alert")
                .Annotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .Annotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment,verification_reward")
                .Annotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .Annotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .Annotation("Npgsql:Enum:verification_step", "email_verification,phone_verification,profile_photo_upload,identity_verification,home_verification")
                .OldAnnotation("Npgsql:Enum:address_type", "primary,secondary,other")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled,completed")
                .OldAnnotation("Npgsql:Enum:cancellation_policy_name", "standard,late")
                .OldAnnotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,change_requested,change_accepted,change_rejected,system_alert")
                .OldAnnotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .OldAnnotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment,verification_reward")
                .OldAnnotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .OldAnnotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .OldAnnotation("Npgsql:Enum:verification_step", "email_verification,phone_verification,profile_photo_upload,identity_verification,home_verification");
        }
    }
}
