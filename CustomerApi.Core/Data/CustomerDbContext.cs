using CustomerApi.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Core.Data;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    public DbSet<CustomerBusinessProfile> CustomerBusinessProfiles => Set<CustomerBusinessProfile>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Status).HasDefaultValue("Active");
            entity.Property(e => e.CustomerType).HasDefaultValue("B2C");
        });

        // CustomerAddress configuration
        modelBuilder.Entity<CustomerAddress>(entity =>
        {
            entity.HasOne(a => a.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CustomerBusinessProfile configuration (one-to-one)
        modelBuilder.Entity<CustomerBusinessProfile>(entity =>
        {
            entity.HasIndex(e => e.CustomerId).IsUnique();
            entity.HasOne(bp => bp.Customer)
                .WithOne(c => c.BusinessProfile)
                .HasForeignKey<CustomerBusinessProfile>(bp => bp.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Tag configuration
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasIndex(e => e.TagName).IsUnique();
        });

        // Many-to-many: Customer <-> Tag (using join table customer_tags)
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Tags)
            .WithMany(t => t.Customers)
            .UsingEntity<Dictionary<string, object>>(
                "CustomerTag",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                j => j.HasOne<Customer>().WithMany().HasForeignKey("CustomerId")
            );

        // Seed data
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice.johnson@acme.com", Phone = "(555) 100-1001", Status = "Active", CustomerType = "B2B", CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 2, FirstName = "Bob", LastName = "Smith", Email = "bob.smith@globex.com", Phone = "(555) 200-2002", Status = "Active", CustomerType = "B2B", CreatedAt = new DateTime(2024, 2, 20, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 3, FirstName = "Carol", LastName = "Williams", Email = "carol.w@initech.com", Phone = "(555) 300-3003", Status = "Inactive", CustomerType = "B2B", CreatedAt = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 4, FirstName = "David", LastName = "Brown", Email = "david.brown@umbrella.com", Phone = "(555) 400-4004", Status = "Active", CustomerType = "B2C", CreatedAt = new DateTime(2024, 4, 5, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 5, FirstName = "Eva", LastName = "Davis", Email = "eva.davis@wayneent.com", Phone = "(555) 500-5005", Status = "Active", CustomerType = "B2B", CreatedAt = new DateTime(2024, 5, 18, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 6, FirstName = "Frank", LastName = "Garcia", Email = "frank.garcia@stark.com", Phone = "(555) 600-6006", Status = "Archived", CustomerType = "B2B", CreatedAt = new DateTime(2024, 6, 22, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 7, FirstName = "Grace", LastName = "Martinez", Email = "grace.m@oscorp.com", Phone = "(555) 700-7007", Status = "Active", CustomerType = "B2C", CreatedAt = new DateTime(2024, 7, 30, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 8, FirstName = "Henry", LastName = "Rodriguez", Email = "henry.r@lexcorp.com", Phone = "(555) 800-8008", Status = "Pending", CustomerType = "B2B", CreatedAt = new DateTime(2024, 8, 12, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 9, FirstName = "Irene", LastName = "Wilson", Email = "irene.wilson@cyberdyne.com", Phone = "(555) 900-9009", Status = "Inactive", CustomerType = "B2C", CreatedAt = new DateTime(2024, 9, 25, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 10, FirstName = "James", LastName = "Anderson", Email = "james.a@weyland.com", Phone = "(555) 101-0101", Status = "Active", CustomerType = "B2B", CreatedAt = new DateTime(2024, 10, 8, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 11, FirstName = "Karen", LastName = "Thomas", Email = "karen.t@soylent.com", Phone = "(555) 111-1111", Status = "Active", CustomerType = "B2C", CreatedAt = new DateTime(2024, 11, 14, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 12, FirstName = "Leo", LastName = "Jackson", Email = "leo.jackson@massive.com", Phone = "(555) 121-2121", Status = "Inactive", CustomerType = "B2B", CreatedAt = new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        // Seed business profiles (company info moved here)
        modelBuilder.Entity<CustomerBusinessProfile>().HasData(
            new CustomerBusinessProfile { Id = 1, CustomerId = 1, CompanyName = "Acme Corp", LifecycleStage = "Customer" },
            new CustomerBusinessProfile { Id = 2, CustomerId = 2, CompanyName = "Globex Inc", LifecycleStage = "Customer" },
            new CustomerBusinessProfile { Id = 3, CustomerId = 3, CompanyName = "Initech", LifecycleStage = "Churned" },
            new CustomerBusinessProfile { Id = 4, CustomerId = 4, CompanyName = "Umbrella LLC", LifecycleStage = "Customer" },
            new CustomerBusinessProfile { Id = 5, CustomerId = 5, CompanyName = "Wayne Enterprises", LifecycleStage = "Customer" },
            new CustomerBusinessProfile { Id = 6, CustomerId = 6, CompanyName = "Stark Industries", LifecycleStage = "Churned" },
            new CustomerBusinessProfile { Id = 7, CustomerId = 7, CompanyName = "Oscorp", LifecycleStage = "Lead" },
            new CustomerBusinessProfile { Id = 8, CustomerId = 8, CompanyName = "LexCorp", LifecycleStage = "Opportunity" },
            new CustomerBusinessProfile { Id = 9, CustomerId = 9, CompanyName = "Cyberdyne Systems", LifecycleStage = "Churned" },
            new CustomerBusinessProfile { Id = 10, CustomerId = 10, CompanyName = "Weyland-Yutani", LifecycleStage = "Customer" },
            new CustomerBusinessProfile { Id = 11, CustomerId = 11, CompanyName = "Soylent Corp", LifecycleStage = "Customer" },
            new CustomerBusinessProfile { Id = 12, CustomerId = 12, CompanyName = "Massive Dynamic", LifecycleStage = "Churned" }
        );

        // Seed tags
        modelBuilder.Entity<Tag>().HasData(
            new Tag { Id = 1, TagName = "VIP" },
            new Tag { Id = 2, TagName = "Enterprise" },
            new Tag { Id = 3, TagName = "Startup" }
        );
    }
}
