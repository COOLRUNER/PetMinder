using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddFlagsConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled")
                .Annotation("Npgsql:Enum:cancellation_policy_name", "standard,late")
                .Annotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .Annotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .Annotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,change_requested,change_accepted,change_rejected,system_alert")
                .Annotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .Annotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment")
                .Annotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .Annotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled")
                .OldAnnotation("Npgsql:Enum:cancellation_policy_name", "standard,late")
                .OldAnnotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,change_requested,change_accepted,change_rejected,system_alert")
                .OldAnnotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .OldAnnotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment")
                .OldAnnotation("Npgsql:Enum:user_role", "admin,sitter,owner,basic_user")
                .OldAnnotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled")
                .Annotation("Npgsql:Enum:cancellation_policy_name", "standard,late")
                .Annotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .Annotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .Annotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,change_requested,change_accepted,change_rejected,system_alert")
                .Annotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .Annotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment")
                .Annotation("Npgsql:Enum:user_role", "admin,sitter,owner,basic_user")
                .Annotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified")
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled")
                .OldAnnotation("Npgsql:Enum:cancellation_policy_name", "standard,late")
                .OldAnnotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,change_requested,change_accepted,change_rejected,system_alert")
                .OldAnnotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .OldAnnotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment")
                .OldAnnotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .OldAnnotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified");
        }
    }
}
