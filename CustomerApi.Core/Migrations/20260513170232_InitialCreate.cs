using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CustomerApi.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Company = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "active"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Company", "CreatedAt", "Email", "FirstName", "LastName", "Phone", "Status" },
                values: new object[,]
                {
                    { 1, "Acme Corp", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "alice.johnson@acme.com", "Alice", "Johnson", "(555) 100-1001", "active" },
                    { 2, "Globex Inc", new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), "bob.smith@globex.com", "Bob", "Smith", "(555) 200-2002", "active" },
                    { 3, "Initech", new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), "carol.w@initech.com", "Carol", "Williams", "(555) 300-3003", "inactive" },
                    { 4, "Umbrella LLC", new DateTime(2024, 4, 5, 0, 0, 0, 0, DateTimeKind.Utc), "david.brown@umbrella.com", "David", "Brown", "(555) 400-4004", "active" },
                    { 5, "Wayne Enterprises", new DateTime(2024, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), "eva.davis@wayneent.com", "Eva", "Davis", "(555) 500-5005", "active" },
                    { 6, "Stark Industries", new DateTime(2024, 6, 22, 0, 0, 0, 0, DateTimeKind.Utc), "frank.garcia@stark.com", "Frank", "Garcia", "(555) 600-6006", "inactive" },
                    { 7, "Oscorp", new DateTime(2024, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), "grace.m@oscorp.com", "Grace", "Martinez", "(555) 700-7007", "active" },
                    { 8, "LexCorp", new DateTime(2024, 8, 12, 0, 0, 0, 0, DateTimeKind.Utc), "henry.r@lexcorp.com", "Henry", "Rodriguez", "(555) 800-8008", "active" },
                    { 9, "Cyberdyne Systems", new DateTime(2024, 9, 25, 0, 0, 0, 0, DateTimeKind.Utc), "irene.wilson@cyberdyne.com", "Irene", "Wilson", "(555) 900-9009", "inactive" },
                    { 10, "Weyland-Yutani", new DateTime(2024, 10, 8, 0, 0, 0, 0, DateTimeKind.Utc), "james.a@weyland.com", "James", "Anderson", "(555) 101-0101", "active" },
                    { 11, "Soylent Corp", new DateTime(2024, 11, 14, 0, 0, 0, 0, DateTimeKind.Utc), "karen.t@soylent.com", "Karen", "Thomas", "(555) 111-1111", "active" },
                    { 12, "Massive Dynamic", new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), "leo.jackson@massive.com", "Leo", "Jackson", "(555) 121-2121", "inactive" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
