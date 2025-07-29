using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "MasterSchema",
                table: "Categories",
                columns: new[] { "Id", "catName", "catOrder", "isDeleted" },
                values: new object[,]
                {
                    { 1, "Science", 1, false },
                    { 2, "Technology", 2, false },
                    { 3, "History", 3, false }
                });

            migrationBuilder.InsertData(
                schema: "MasterSchema",
                table: "Products",
                columns: new[] { "Id", "Author", "CategoryId", "Description", "BookPrice", "Title" },
                values: new object[,]
                {
                    { 1, "Author 1", 1, "Desc 1", 99.99m, "Book 1" },
                    { 2, "Author 2", 2, "Desc 2", 49.50m, "Book 2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "MasterSchema",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "MasterSchema",
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "MasterSchema",
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "MasterSchema",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "MasterSchema",
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
