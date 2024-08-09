using EmployeeAPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAPI.Data
{
    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseSqlServer(
                @"Server=CPH00301;Database=ExternalEmployeeAPI;Integrated Security=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(new List<Employee>
            {
                new()
                {
                    Id = 1, Email = "John@test.com", ImageUrl = "testurl.com", JobTitle = "Associate QA Tester",
                    Location = "Copenhagen", Name = "John Johnson"
                },
                new()
                {
                    Id = 2, Email = "Jane@test.com", ImageUrl = "testurl2.com", JobTitle = "Senior Developer",
                    Location = "Oslo", Name = "Jane Johnson"
                },
                new()
                {
                    Id = 3, Email = "Bob@test.com", ImageUrl = "testurl3.com", JobTitle = "Project Manager",
                    Location = "Oslo", Name = "Bob Johnson"
                },
                new()
                {
                    Id = 4, Email = "Alice@test.com", ImageUrl = "testurl4.com", JobTitle = "Product Owner",
                    Location = "Copenhagen", Name = "Alice Johnson"
                },
                new()
                {
                    Id = 5, Email = "Charlie@test.com", ImageUrl = "testurl5.com", JobTitle = "Scrum Master",
                    Location = "Copenhagen", Name = "Charlie Johnson"
                },
                new()
                {
                    Id = 6, Email = "David@test.com", ImageUrl = "testurl6.com", JobTitle = "Software Engineer",
                    Location = "London", Name = "David Johnson"
                },
                new()
                {
                    Id = 7, Email = "Eva@test.com", ImageUrl = "testurl7.com", JobTitle = "UX Designer",
                    Location = "London", Name = "Eva Johnson"
                },
                new()
                {
                    Id = 8, Email = "Frank@test.com", ImageUrl = "testurl8.com", JobTitle = "Data Analyst",
                    Location = "London", Name = "Frank Johnson"
                },
                new()
                {
                    Id = 9, Email = "Grace@test.com", ImageUrl = "testurl9.com", JobTitle = "Business Analyst",
                    Location = "Copenhagen", Name = "Grace Johnson"
                },
                new()
                {
                    Id = 10, Email = "Harry@test.com", ImageUrl = "testurl10.com", JobTitle = "DevOps Engineer",
                    Location = "Copenhagen", Name = "Harry Johnson"
                },
                new()
                {
                    Id = 11, Email = "Irene@test.com", ImageUrl = "testurl11.com", JobTitle = "Software Tester",
                    Location = "Copenhagen", Name = "Irene Johnson"
                },
                new()
                {
                    Id = 12, Email = "Jack@test.com", ImageUrl = "testurl12.com", JobTitle = "Frontend Developer",
                    Location = "Oslo", Name = "Jack Johnson"
                },
                new()
                {
                    Id = 13, Email = "Kelly@test.com", ImageUrl = "testurl13.com", JobTitle = "Backend Developer",
                    Location = "Copenhagen", Name = "Kelly Johnson"
                },
                new()
                {
                    Id = 14, Email = "Larry@test.com", ImageUrl = "testurl14.com", JobTitle = "Database Administrator",
                    Location = "Washington D.C.", Name = "Larry Johnson"
                },
                new()
                {
                    Id = 15, Email = "Mona@test.com", ImageUrl = "testurl15.com", JobTitle = "System Analyst",
                    Location = "Washington D.C.", Name = "Mona Johnson"
                },
                new()
                {
                    Id = 16, Email = "Nancy@test.com", ImageUrl = "testurl16.com", JobTitle = "Network Engineer",
                    Location = "Washington D.C.", Name = "Nancy Johnson"
                },
                new()
                {
                    Id = 17, Email = "Oscar@test.com", ImageUrl = "testurl17.com", JobTitle = "Security Analyst",
                    Location = "New York", Name = "Oscar Johnson"
                },
                new()
                {
                    Id = 18, Email = "Paul@test.com", ImageUrl = "testurl18.com", JobTitle = "Full Stack Developer",
                    Location = "New York", Name = "Paul Johnson"
                },
                new()
                {
                    Id = 19, Email = "Quincy@test.com", ImageUrl = "testurl19.com", JobTitle = "Data Scientist",
                    Location = "New York", Name = "Quincy Johnson"
                },
                new()
                {
                    Id = 20, Email = "Rita@test.com", ImageUrl = "testurl20.com", JobTitle = "Mobile App Developer",
                    Location = "Copenhagen", Name = "Rita Johnson"
                }
                // Add more test users here
            });
        }

        public DbSet<Employee> Employees { get; set; }
    }
}