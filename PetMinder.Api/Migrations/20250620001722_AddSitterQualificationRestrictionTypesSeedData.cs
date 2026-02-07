using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddSitterQualificationRestrictionTypesSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "QualificationTypes",
                columns: new[] { "QualificationTypeId", "Code", "Description" },
                values: new object[,]
                {
                    { 1L, "VET_EXP", "Professional experience in veterinary care." },
                    { 2L, "FIRST_AID", "Certified in pet first aid and CPR." },
                    { 3L, "DOG_TRAINER", "Experience with dog training techniques." },
                    { 4L, "CAT_SPECIALIST", "Specialized knowledge in cat behavior." }
                });

            migrationBuilder.InsertData(
                table: "RestrictionTypes",
                columns: new[] { "RestrictionTypeId", "Code", "Description" },
                values: new object[,]
                {
                    { 1L, "NO_LARGE_DOGS", "Cannot care for dogs weighing over 50 lbs." },
                    { 2L, "NO_CATS", "Allergic to or cannot care for cats." },
                    { 3L, "NO_UNVACCINATED", "Only cares for pets with up-to-date vaccinations." },
                    { 4L, "NO_OVERNIGHT", "Only offers daytime services, no overnight care." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "QualificationTypes",
                keyColumn: "QualificationTypeId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "QualificationTypes",
                keyColumn: "QualificationTypeId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "QualificationTypes",
                keyColumn: "QualificationTypeId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "QualificationTypes",
                keyColumn: "QualificationTypeId",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "RestrictionTypes",
                keyColumn: "RestrictionTypeId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "RestrictionTypes",
                keyColumn: "RestrictionTypeId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "RestrictionTypes",
                keyColumn: "RestrictionTypeId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "RestrictionTypes",
                keyColumn: "RestrictionTypeId",
                keyValue: 4L);
        }
    }
}
