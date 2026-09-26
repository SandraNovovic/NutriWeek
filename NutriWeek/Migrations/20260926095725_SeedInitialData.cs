using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NutriWeek.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "Calories", "Description", "DishType", "ImageUrl", "Ingredients", "Instructions", "Name", "Portions", "PreparationTime" },
                values: new object[,]
                {
                    { 1, 520, "A balanced meal with chicken, rice and vegetables.", 0, null, "Chicken breast, rice, tomato, cucumber, olive oil", "Cook the rice. Cook the chicken. Chop the vegetables and serve everything together.", "Chicken Rice Bowl", 2, 30 },
                    { 2, 430, "Pasta with vegetables and tomato sauce.", 2, null, "Pasta, tomato sauce, zucchini, bell pepper, parmesan", "Cook the pasta. Cook the vegetables and tomato sauce. Mix everything together.", "Vegetarian Pasta", 2, 25 },
                    { 3, 480, "Baked salmon served with fresh vegetables.", 1, null, "Salmon, broccoli, carrots, olive oil, lemon", "Season the salmon and vegetables. Bake until cooked and serve with lemon.", "Salmon with Vegetables", 2, 35 },
                    { 4, 350, "Oatmeal with banana and fresh berries.", 3, null, "Oats, oat milk, banana, blueberries, strawberries", "Cook the oats with oat milk. Add sliced banana and berries before serving.", "Vegan Oatmeal", 1, 10 }
                });

            migrationBuilder.InsertData(
                table: "WeeklyMenus",
                columns: new[] { "Id", "Title", "WeekStartDate" },
                values: new object[] { 1, "Healthy Week", new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "DailyMenus",
                columns: new[] { "Id", "Date", "WeeklyMenuId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, new DateTime(2026, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "Meals",
                columns: new[] { "Id", "DailyMenuId", "DishId", "MealType" },
                values: new object[,]
                {
                    { 1, 1, 4, 0 },
                    { 2, 1, 1, 1 },
                    { 3, 1, 3, 2 },
                    { 4, 2, 4, 0 },
                    { 5, 2, 2, 1 },
                    { 6, 2, 1, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "DailyMenus",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DailyMenus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Dishes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "WeeklyMenus",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
