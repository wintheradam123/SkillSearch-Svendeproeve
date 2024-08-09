using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExternalEmployeeAPI.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Email", "ImageUrl", "JobTitle", "Location", "Name" },
                values: new object[,]
                {
                    { 1, "John@test.com", "testurl.com", "Associate QA Tester", "Copenhagen", "John Johnson" },
                    { 2, "Jane@test.com", "testurl2.com", "Senior Developer", "Copenhagen", "Jane Johnson" },
                    { 3, "Bob@test.com", "testurl3.com", "Project Manager", "Copenhagen", "Bob Johnson" },
                    { 4, "Alice@test.com", "testurl4.com", "Product Owner", "Copenhagen", "Alice Johnson" },
                    { 5, "Charlie@test.com", "testurl5.com", "Scrum Master", "Copenhagen", "Charlie Johnson" },
                    { 6, "David@test.com", "testurl6.com", "Software Engineer", "Copenhagen", "David Johnson" },
                    { 7, "Eva@test.com", "testurl7.com", "UX Designer", "Copenhagen", "Eva Johnson" },
                    { 8, "Frank@test.com", "testurl8.com", "Data Analyst", "Copenhagen", "Frank Johnson" },
                    { 9, "Grace@test.com", "testurl9.com", "Business Analyst", "Copenhagen", "Grace Johnson" },
                    { 10, "Harry@test.com", "testurl10.com", "DevOps Engineer", "Copenhagen", "Harry Johnson" },
                    { 11, "Irene@test.com", "testurl11.com", "Software Tester", "Copenhagen", "Irene Johnson" },
                    { 12, "Jack@test.com", "testurl12.com", "Frontend Developer", "Copenhagen", "Jack Johnson" },
                    { 13, "Kelly@test.com", "testurl13.com", "Backend Developer", "Copenhagen", "Kelly Johnson" },
                    { 14, "Larry@test.com", "testurl14.com", "Database Administrator", "Copenhagen", "Larry Johnson" },
                    { 15, "Mona@test.com", "testurl15.com", "System Analyst", "Copenhagen", "Mona Johnson" },
                    { 16, "Nancy@test.com", "testurl16.com", "Network Engineer", "Copenhagen", "Nancy Johnson" },
                    { 17, "Oscar@test.com", "testurl17.com", "Security Analyst", "Copenhagen", "Oscar Johnson" },
                    { 18, "Paul@test.com", "testurl18.com", "Full Stack Developer", "Copenhagen", "Paul Johnson" },
                    { 19, "Quincy@test.com", "testurl19.com", "Data Scientist", "Copenhagen", "Quincy Johnson" },
                    { 20, "Rita@test.com", "testurl20.com", "Mobile App Developer", "Copenhagen", "Rita Johnson" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
