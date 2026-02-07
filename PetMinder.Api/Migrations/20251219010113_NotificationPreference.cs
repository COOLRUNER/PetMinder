using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class NotificationPreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:address_type", "primary,secondary,other")
                .Annotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled,completed")
                .Annotation("Npgsql:Enum:cancellation_policy_name", "standard,late,in_progress")
                .Annotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .Annotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .Annotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,booking_completed,change_requested,change_accepted,change_rejected,system_alert")
                .Annotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .Annotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment,verification_reward,expiration,referral_bonus,achievement_bonus")
                .Annotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .Annotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .Annotation("Npgsql:Enum:verification_step", "email_verification,phone_verification,profile_photo_upload,identity_verification,home_verification")
                .OldAnnotation("Npgsql:Enum:address_type", "primary,secondary,other")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled,completed")
                .OldAnnotation("Npgsql:Enum:cancellation_policy_name", "standard,late,in_progress")
                .OldAnnotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,booking_completed,change_requested,change_accepted,change_rejected,system_alert")
                .OldAnnotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .OldAnnotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment,verification_reward,expiration")
                .OldAnnotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .OldAnnotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .OldAnnotation("Npgsql:Enum:verification_step", "email_verification,phone_verification,profile_photo_upload,identity_verification,home_verification");

            migrationBuilder.CreateTable(
                name: "NotificationPreferences",
                columns: table => new
                {
                    PreferenceId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    NotificationType = table.Column<int>(type: "integer", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPreferences", x => x.PreferenceId);
                    table.ForeignKey(
                        name: "FK_NotificationPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Use an idempotent raw SQL insert for Postgres so applying migrations twice won't fail when badges already exist.
            migrationBuilder.Sql(@"INSERT INTO ""Badges"" (""BadgeId"", ""Description"", ""IsSpendable"", ""Name"") VALUES
                (1, 'Completed 5+ bookings in a row without cancellations.', false, 'Reliable Sitter Streak'),
                (2, 'Provided pet sitting services during high-demand holidays.', false, 'Peak-Season Helper'),
                (3, 'Maintains an average rating of 4.8+ with at least 10 reviews.', false, 'Top Rated'),
                (4, 'Completed 10 or more bookings.', false, 'Frequent Sitter'),
                (5, 'Contributed 5 or more reviews to the community.', false, 'Reviewer')
            ON CONFLICT (""BadgeId"") DO NOTHING;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationPreferences");

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "BadgeId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "BadgeId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "BadgeId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "BadgeId",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Badges",
                keyColumn: "BadgeId",
                keyValue: 5L);

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:address_type", "primary,secondary,other")
                .Annotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled,completed")
                .Annotation("Npgsql:Enum:cancellation_policy_name", "standard,late,in_progress")
                .Annotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .Annotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .Annotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,booking_completed,change_requested,change_accepted,change_rejected,system_alert")
                .Annotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .Annotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment,verification_reward,expiration")
                .Annotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .Annotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .Annotation("Npgsql:Enum:verification_step", "email_verification,phone_verification,profile_photo_upload,identity_verification,home_verification")
                .OldAnnotation("Npgsql:Enum:address_type", "primary,secondary,other")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled,completed")
                .OldAnnotation("Npgsql:Enum:cancellation_policy_name", "standard,late,in_progress")
                .OldAnnotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,booking_completed,change_requested,change_accepted,change_rejected,system_alert")
                .OldAnnotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .OldAnnotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment,verification_reward,expiration,referral_bonus,achievement_bonus")
                .OldAnnotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .OldAnnotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .OldAnnotation("Npgsql:Enum:verification_step", "email_verification,phone_verification,profile_photo_upload,identity_verification,home_verification");
        }
    }
}
