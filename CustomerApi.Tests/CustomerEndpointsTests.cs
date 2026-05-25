using CustomerApi.Core.Data;
using CustomerApi.Core.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;

namespace CustomerApi.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "IntegrationTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Remove all EF Core DbContext registrations
            services.RemoveAll(typeof(DbContextOptions<CustomerDbContext>));
            services.RemoveAll(typeof(CustomerDbContext));

            // Create an isolated InMemory service provider to avoid dual-provider conflict
            var efServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<CustomerDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
                options.UseInternalServiceProvider(efServiceProvider);
            });

            // Build and seed
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
            db.Database.EnsureCreated();
        });
    }
}

public class CustomerEndpointsTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CustomerEndpointsTests()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    // ----- GET /api/customers -----

    [Fact]
    public async Task GetAll_Returns200WithCustomerList()
    {
        var response = await _client.GetAsync("/api/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();
        customers.Should().NotBeNull();
        customers!.Count.Should().Be(12);
    }

    [Fact]
    public async Task GetAll_ReturnsCorrectFirstCustomer()
    {
        var customers = await _client.GetFromJsonAsync<List<CustomerDto>>("/api/customers");

        customers.Should().NotBeNull();
        var first = customers!.First(c => c.Id == 1);
        first.FirstName.Should().Be("Alice");
        first.LastName.Should().Be("Johnson");
        first.Email.Should().Be("alice.johnson@acme.com");
    }

    // ----- GET /api/customers/{id} -----

    [Fact]
    public async Task GetById_ExistingId_Returns200()
    {
        var response = await _client.GetAsync("/api/customers/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task GetById_NonExistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/customers/9999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ----- POST /api/customers -----

    [Fact]
    public async Task Create_ValidData_Returns201()
    {
        var newCustomer = new CreateCustomerDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test.user@example.com",
            Phone = "(555) 999-9999",
            CompanyName = "Test Corp",
            Status = "Active",
            CustomerType = "B2C"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", newCustomer);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<CustomerDto>();
        created.Should().NotBeNull();
        created!.FirstName.Should().Be("Test");
        created.Id.Should().BeGreaterThan(0);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_MissingRequiredFields_Returns400()
    {
        var invalid = new { FirstName = "", Email = "not-valid" };

        var response = await _client.PostAsJsonAsync("/api/customers", invalid);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ----- PUT /api/customers/{id} -----

    [Fact]
    public async Task Update_ExistingId_Returns200()
    {
        var update = new UpdateCustomerDto
        {
            FirstName = "UpdatedAlice",
            CompanyName = "Updated Corp"
        };

        var response = await _client.PutAsJsonAsync("/api/customers/1", update);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<CustomerDto>();
        updated.Should().NotBeNull();
        updated!.FirstName.Should().Be("UpdatedAlice");
    }

    [Fact]
    public async Task Update_NonExistentId_Returns404()
    {
        var update = new UpdateCustomerDto { FirstName = "Ghost" };

        var response = await _client.PutAsJsonAsync("/api/customers/9999", update);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ----- DELETE /api/customers/{id} -----

    [Fact]
    public async Task Delete_ExistingId_Returns204()
    {
        var response = await _client.DeleteAsync("/api/customers/12");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_NonExistentId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/customers/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
