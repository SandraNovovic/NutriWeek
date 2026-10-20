using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutriWeek.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeWeeklyMenuIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyMenus_WeeklyMenus_WeeklyMenuId",
                table: "DailyMenus");

            migrationBuilder.AlterColumn<int>(
                name: "WeeklyMenuId",
                table: "DailyMenus",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyMenus_WeeklyMenus_WeeklyMenuId",
                table: "DailyMenus",
                column: "WeeklyMenuId",
                principalTable: "WeeklyMenus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyMenus_WeeklyMenus_WeeklyMenuId",
                table: "DailyMenus");

            migrationBuilder.AlterColumn<int>(
                name: "WeeklyMenuId",
                table: "DailyMenus",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyMenus_WeeklyMenus_WeeklyMenuId",
                table: "DailyMenus",
                column: "WeeklyMenuId",
                principalTable: "WeeklyMenus",
                principalColumn: "Id");
        }
    }
}
