using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddUserVerificationStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserVerificationStep_Users_UserId",
                table: "UserVerificationStep");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserVerificationStep",
                table: "UserVerificationStep");

            migrationBuilder.DropIndex(
                name: "IX_UserVerificationStep_UserId",
                table: "UserVerificationStep");

            migrationBuilder.RenameTable(
                name: "UserVerificationStep",
                newName: "UserVerificationSteps");

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
                .OldAnnotation("Npgsql:Enum:booking_status", "pending,accepted,rejected,cancelled,completed")
                .OldAnnotation("Npgsql:Enum:cancellation_policy_name", "standard,late")
                .OldAnnotation("Npgsql:Enum:cancelled_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:change_requested_by", "owner,sitter")
                .OldAnnotation("Npgsql:Enum:notification_type", "booking_request,booking_accepted,booking_rejected,booking_cancelled,change_requested,change_accepted,change_rejected,system_alert")
                .OldAnnotation("Npgsql:Enum:point_policy_service_type", "pet,house")
                .OldAnnotation("Npgsql:Enum:transaction_type", "booking,donation,adjustment,verification_reward")
                .OldAnnotation("Npgsql:Enum:user_role", "none,basic_user,owner,sitter,admin")
                .OldAnnotation("Npgsql:Enum:verification_status_name", "unverified,basic,verified,home_verified");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserVerificationSteps",
                table: "UserVerificationSteps",
                column: "UserVerificationStepId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVerificationSteps_UserId_Step",
                table: "UserVerificationSteps",
                columns: new[] { "UserId", "Step" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserVerificationSteps_Users_UserId",
                table: "UserVerificationSteps",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserVerificationSteps_Users_UserId",
                table: "UserVerificationSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserVerificationSteps",
                table: "UserVerificationSteps");

            migrationBuilder.DropIndex(
                name: "IX_UserVerificationSteps_UserId_Step",
                table: "UserVerificationSteps");

            migrationBuilder.RenameTable(
                name: "UserVerificationSteps",
                newName: "UserVerificationStep");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserVerificationStep",
                table: "UserVerificationStep",
                column: "UserVerificationStepId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVerificationStep_UserId",
                table: "UserVerificationStep",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserVerificationStep_Users_UserId",
                table: "UserVerificationStep",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
