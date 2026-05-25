using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CustomerApi.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Customers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "active");

            migrationBuilder.AddColumn<string>(
                name: "CustomerType",
                table: "Customers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "B2C");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    AddressType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AddressLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StateProvince = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerBusinessProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LeadSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LifecycleStage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerBusinessProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerBusinessProfiles_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerTag",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTag", x => new { x.CustomerId, x.TagId });
                    table.ForeignKey(
                        name: "FK_CustomerTag_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CustomerBusinessProfiles",
                columns: new[] { "Id", "CompanyName", "CustomerId", "JobTitle", "LeadSource", "LifecycleStage" },
                values: new object[,]
                {
                    { 1, "Acme Corp", 1, null, null, "Customer" },
                    { 2, "Globex Inc", 2, null, null, "Customer" },
                    { 3, "Initech", 3, null, null, "Churned" },
                    { 4, "Umbrella LLC", 4, null, null, "Customer" },
                    { 5, "Wayne Enterprises", 5, null, null, "Customer" },
                    { 6, "Stark Industries", 6, null, null, "Churned" },
                    { 7, "Oscorp", 7, null, null, "Lead" },
                    { 8, "LexCorp", 8, null, null, "Opportunity" },
                    { 9, "Cyberdyne Systems", 9, null, null, "Churned" },
                    { 10, "Weyland-Yutani", 10, null, null, "Customer" },
                    { 11, "Soylent Corp", 11, null, null, "Customer" },
                    { 12, "Massive Dynamic", 12, null, null, "Churned" }
                });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CustomerType", "Status", "UpdatedAt" },
                values: new object[] { "B2B", "Active", null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CustomerType", "Status", "UpdatedAt" },
                values: new object[] { "B2B", "Active", null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CustomerType", "Status", "UpdatedAt" },
                values: new object[] { "B2B", "Inactive", null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CustomerType", "Status", "UpdatedAt" },
                values: new object[] { "B2C", "Active", null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CustomerType", "Status", "UpdatedAt" },
                values: new object[] { "B2B", "Active", null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CustomerType", "Status", "UpdatedAt" },
                values: new object[] { "B2B", "Archived", null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CustomerType", "Status", "UpdatedAt" },
                values: new object[] { "B2C", "Active", null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CustomerType", "Status", "UpdatedAt" },
                values: new object[] { "B2B", "Pending", null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CustomerType", "Status", "UpdatedAt" },
                values: new object[] { "B2C", "Inactive", null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CustomerType", "Status", "UpdatedAt" },
                values: new object[] { "B2B", "Active", null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CustomerType", "Status", "UpdatedAt" },
                values: new object[] { "B2C", "Active", null });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CustomerType", "Status", "UpdatedAt" },
                values: new object[] { "B2B", "Inactive", null });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "TagName" },
                values: new object[,]
                {
                    { 1, "VIP" },
                    { 2, "Enterprise" },
                    { 3, "Startup" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_CustomerId",
                table: "CustomerAddresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBusinessProfiles_CustomerId",
                table: "CustomerBusinessProfiles",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTag_TagId",
                table: "CustomerTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagName",
                table: "Tags",
                column: "TagName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerAddresses");

            migrationBuilder.DropTable(
                name: "CustomerBusinessProfiles");

            migrationBuilder.DropTable(
                name: "CustomerTag");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropColumn(
                name: "CustomerType",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Customers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "active",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Active");

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "Customers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Company", "Status" },
                values: new object[] { "Acme Corp", "active" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Company", "Status" },
                values: new object[] { "Globex Inc", "active" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Company", "Status" },
                values: new object[] { "Initech", "inactive" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Company", "Status" },
                values: new object[] { "Umbrella LLC", "active" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Company", "Status" },
                values: new object[] { "Wayne Enterprises", "active" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Company", "Status" },
                values: new object[] { "Stark Industries", "inactive" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Company", "Status" },
                values: new object[] { "Oscorp", "active" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Company", "Status" },
                values: new object[] { "LexCorp", "active" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Company", "Status" },
                values: new object[] { "Cyberdyne Systems", "inactive" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Company", "Status" },
                values: new object[] { "Weyland-Yutani", "active" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Company", "Status" },
                values: new object[] { "Soylent Corp", "active" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Company", "Status" },
                values: new object[] { "Massive Dynamic", "inactive" });
        }
    }
}
