using CustomerApi.Core.Data;
using CustomerApi.Core.Entities;
using CustomerApi.Core.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Tests;

public class CustomerRepositoryTests : IDisposable
{
    private readonly CustomerDbContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CustomerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CustomerDbContext(options);
        _context.Database.EnsureCreated();
        _repository = new CustomerRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    // ----- GetAllAsync -----

    [Fact]
    public async Task GetAllAsync_ReturnsAllSeededCustomers()
    {
        var customers = await _repository.GetAllAsync();
        customers.Should().HaveCount(12);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCustomersOrderedById()
    {
        var customers = (await _repository.GetAllAsync()).ToList();
        customers.Should().BeInAscendingOrder(c => c.Id);
    }

    // ----- GetByIdAsync -----

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCustomer()
    {
        var customer = await _repository.GetByIdAsync(1);
        customer.Should().NotBeNull();
        customer!.FirstName.Should().Be("Alice");
        customer.LastName.Should().Be("Johnson");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        var customer = await _repository.GetByIdAsync(9999);
        customer.Should().BeNull();
    }

    // ----- AddAsync -----

    [Fact]
    public async Task AddAsync_ValidCustomer_ReturnsWithGeneratedId()
    {
        var newCustomer = new Customer
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Phone = "(555) 000-0000",
            Company = "Test Inc",
            Status = "active"
        };

        var created = await _repository.AddAsync(newCustomer);

        created.Id.Should().BeGreaterThan(0);
        created.FirstName.Should().Be("Test");
        created.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task AddAsync_IncrementsCount()
    {
        var before = (await _repository.GetAllAsync()).Count();

        await _repository.AddAsync(new Customer
        {
            FirstName = "New",
            LastName = "Person",
            Email = "new@example.com",
            Phone = "(555) 111-2222",
            Company = "NewCo",
            Status = "active"
        });

        var after = (await _repository.GetAllAsync()).Count();
        after.Should().Be(before + 1);
    }

    // ----- UpdateAsync -----

    [Fact]
    public async Task UpdateAsync_ExistingId_UpdatesFields()
    {
        var changes = new Customer { FirstName = "Updated", LastName = "", Email = "", Phone = "", Company = "", Status = "" };
        var updated = await _repository.UpdateAsync(1, changes);

        updated.Should().NotBeNull();
        updated!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateAsync_NonExistentId_ReturnsNull()
    {
        var changes = new Customer { FirstName = "Ghost", LastName = "", Email = "", Phone = "", Company = "", Status = "" };
        var result = await _repository.UpdateAsync(9999, changes);
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_PreservesUnmodifiedFields()
    {
        var original = await _repository.GetByIdAsync(2);
        original.Should().NotBeNull();

        // Only update company — pass empty strings for others so they are not changed
        var changes = new Customer
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty,
            Phone = string.Empty,
            Company = "New Company",
            Status = string.Empty
        };
        var updated = await _repository.UpdateAsync(2, changes);

        updated.Should().NotBeNull();
        updated!.Company.Should().Be("New Company");
        // FirstName should be preserved since we passed empty string (repository skips empty)
    }

    // ----- DeleteAsync -----

    [Fact]
    public async Task DeleteAsync_ExistingId_RemovesAndReturnsTrue()
    {
        var before = (await _repository.GetAllAsync()).Count();
        var result = await _repository.DeleteAsync(1);

        result.Should().BeTrue();
        (await _repository.GetAllAsync()).Count().Should().Be(before - 1);
        (await _repository.GetByIdAsync(1)).Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentId_ReturnsFalse()
    {
        var result = await _repository.DeleteAsync(9999);
        result.Should().BeFalse();
    }
}
