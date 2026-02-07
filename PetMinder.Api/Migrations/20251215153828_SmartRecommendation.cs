using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class SmartRecommendation : Migration
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
                .Annotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment,verification_reward,expiration")
                .Annotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .Annotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .Annotation("Npgsql:Enum:verification_step", "email_verification,phone_verification,profile_photo_upload,identity_verification,home_verification")
                .OldAnnotation("Npgsql:Enum:address_type", "primary,secondary,other")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled,completed")
                .OldAnnotation("Npgsql:Enum:cancellation_policy_name", "standard,late,in_progress")
                .OldAnnotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,change_requested,change_accepted,change_rejected,system_alert")
                .OldAnnotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .OldAnnotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment,verification_reward,expiration")
                .OldAnnotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .OldAnnotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .OldAnnotation("Npgsql:Enum:verification_step", "email_verification,phone_verification,profile_photo_upload,identity_verification,home_verification");

            migrationBuilder.AddColumn<int>(
                name: "BehaviorComplexity",
                table: "Pets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Pets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BehaviorComplexity",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Pets");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:address_type", "primary,secondary,other")
                .Annotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled,completed")
                .Annotation("Npgsql:Enum:cancellation_policy_name", "standard,late,in_progress")
                .Annotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .Annotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .Annotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,change_requested,change_accepted,change_rejected,system_alert")
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
                .OldAnnotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment,verification_reward,expiration")
                .OldAnnotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .OldAnnotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .OldAnnotation("Npgsql:Enum:verification_step", "email_verification,phone_verification,profile_photo_upload,identity_verification,home_verification");
        }
    }
}
