using CustomerApi.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Core.Data;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Status).HasDefaultValue("active");
        });

        // Seed data matching Angular mock-customers.ts
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice.johnson@acme.com", Phone = "(555) 100-1001", Company = "Acme Corp", Status = "active", CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 2, FirstName = "Bob", LastName = "Smith", Email = "bob.smith@globex.com", Phone = "(555) 200-2002", Company = "Globex Inc", Status = "active", CreatedAt = new DateTime(2024, 2, 20, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 3, FirstName = "Carol", LastName = "Williams", Email = "carol.w@initech.com", Phone = "(555) 300-3003", Company = "Initech", Status = "inactive", CreatedAt = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 4, FirstName = "David", LastName = "Brown", Email = "david.brown@umbrella.com", Phone = "(555) 400-4004", Company = "Umbrella LLC", Status = "active", CreatedAt = new DateTime(2024, 4, 5, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 5, FirstName = "Eva", LastName = "Davis", Email = "eva.davis@wayneent.com", Phone = "(555) 500-5005", Company = "Wayne Enterprises", Status = "active", CreatedAt = new DateTime(2024, 5, 18, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 6, FirstName = "Frank", LastName = "Garcia", Email = "frank.garcia@stark.com", Phone = "(555) 600-6006", Company = "Stark Industries", Status = "inactive", CreatedAt = new DateTime(2024, 6, 22, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 7, FirstName = "Grace", LastName = "Martinez", Email = "grace.m@oscorp.com", Phone = "(555) 700-7007", Company = "Oscorp", Status = "active", CreatedAt = new DateTime(2024, 7, 30, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 8, FirstName = "Henry", LastName = "Rodriguez", Email = "henry.r@lexcorp.com", Phone = "(555) 800-8008", Company = "LexCorp", Status = "active", CreatedAt = new DateTime(2024, 8, 12, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 9, FirstName = "Irene", LastName = "Wilson", Email = "irene.wilson@cyberdyne.com", Phone = "(555) 900-9009", Company = "Cyberdyne Systems", Status = "inactive", CreatedAt = new DateTime(2024, 9, 25, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 10, FirstName = "James", LastName = "Anderson", Email = "james.a@weyland.com", Phone = "(555) 101-0101", Company = "Weyland-Yutani", Status = "active", CreatedAt = new DateTime(2024, 10, 8, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 11, FirstName = "Karen", LastName = "Thomas", Email = "karen.t@soylent.com", Phone = "(555) 111-1111", Company = "Soylent Corp", Status = "active", CreatedAt = new DateTime(2024, 11, 14, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 12, FirstName = "Leo", LastName = "Jackson", Email = "leo.jackson@massive.com", Phone = "(555) 121-2121", Company = "Massive Dynamic", Status = "inactive", CreatedAt = new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
