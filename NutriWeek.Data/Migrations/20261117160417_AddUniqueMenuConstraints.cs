using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriWeek.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueMenuConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyMenus_WeeklyMenuId",
                table: "DailyMenus");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "WeeklyMenus",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WeeklyMenus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Meals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Dishes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DailyMenus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsDeleted",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyMenus_UserId_WeekStartDate",
                table: "WeeklyMenus",
                columns: new[] { "UserId", "WeekStartDate" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMenus_WeeklyMenuId_Date",
                table: "DailyMenus",
                columns: new[] { "WeeklyMenuId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WeeklyMenus_UserId_WeekStartDate",
                table: "WeeklyMenus");

            migrationBuilder.DropIndex(
                name: "IX_DailyMenus_WeeklyMenuId_Date",
                table: "DailyMenus");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WeeklyMenus");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Meals");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DailyMenus");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "WeeklyMenus",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyMenus_WeeklyMenuId",
                table: "DailyMenus",
                column: "WeeklyMenuId");
        }
    }
}
