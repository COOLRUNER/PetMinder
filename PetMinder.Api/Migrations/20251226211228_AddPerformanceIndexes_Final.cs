using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes_Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SitterAvailabilities_SitterId",
                table: "SitterAvailabilities");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_RevieweeId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_ReviewReports_ReviewId",
                table: "ReviewReports");

            migrationBuilder.DropIndex(
                name: "IX_PointsTransactions_ReceiverId",
                table: "PointsTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PointsTransactions_SenderId",
                table: "PointsTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_OwnerId",
                table: "BookingRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_SitterId",
                table: "BookingRequests");

            migrationBuilder.CreateIndex(
                name: "IX_SitterAvailabilities_SitterId_StartTime_EndTime",
                table: "SitterAvailabilities",
                columns: new[] { "SitterId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RevieweeId_CreatedAt",
                table: "Reviews",
                columns: new[] { "RevieweeId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId_BookingId",
                table: "Reviews",
                columns: new[] { "ReviewerId", "BookingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewReports_ReviewId_ReporterId",
                table: "ReviewReports",
                columns: new[] { "ReviewId", "ReporterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PointsTransactions_ReceiverId_OccurredAt",
                table: "PointsTransactions",
                columns: new[] { "ReceiverId", "OccurredAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_PointsTransactions_SenderId_OccurredAt",
                table: "PointsTransactions",
                columns: new[] { "SenderId", "OccurredAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_PointsLots_UserId_IsExpired_PointsRemaining_ExpiresAt",
                table: "PointsLots",
                columns: new[] { "UserId", "IsExpired", "PointsRemaining", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_CreatedAt",
                table: "Notifications",
                columns: new[] { "UserId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId_SentAt",
                table: "Messages",
                columns: new[] { "ConversationId", "SentAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_OwnerId_CreatedAt",
                table: "BookingRequests",
                columns: new[] { "OwnerId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_SitterId_CreatedAt",
                table: "BookingRequests",
                columns: new[] { "SitterId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_SitterId_Status_StartTime_EndTime",
                table: "BookingRequests",
                columns: new[] { "SitterId", "Status", "StartTime", "EndTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SitterAvailabilities_SitterId_StartTime_EndTime",
                table: "SitterAvailabilities");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_RevieweeId_CreatedAt",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ReviewerId_BookingId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_ReviewReports_ReviewId_ReporterId",
                table: "ReviewReports");

            migrationBuilder.DropIndex(
                name: "IX_PointsTransactions_ReceiverId_OccurredAt",
                table: "PointsTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PointsTransactions_SenderId_OccurredAt",
                table: "PointsTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PointsLots_UserId_IsExpired_PointsRemaining_ExpiresAt",
                table: "PointsLots");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId_CreatedAt",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ConversationId_SentAt",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_OwnerId_CreatedAt",
                table: "BookingRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_SitterId_CreatedAt",
                table: "BookingRequests");

            migrationBuilder.DropIndex(
                name: "IX_BookingRequests_SitterId_Status_StartTime_EndTime",
                table: "BookingRequests");

            migrationBuilder.CreateIndex(
                name: "IX_SitterAvailabilities_SitterId",
                table: "SitterAvailabilities",
                column: "SitterId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RevieweeId",
                table: "Reviews",
                column: "RevieweeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewReports_ReviewId",
                table: "ReviewReports",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsTransactions_ReceiverId",
                table: "PointsTransactions",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsTransactions_SenderId",
                table: "PointsTransactions",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_OwnerId",
                table: "BookingRequests",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_SitterId",
                table: "BookingRequests",
                column: "SitterId");
        }
    }
}
