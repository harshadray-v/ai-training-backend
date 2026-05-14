using System.ComponentModel.DataAnnotations;
using CustomerApi.Core.Data;
using CustomerApi.Core.DTOs;
using CustomerApi.Core.Entities;
using CustomerApi.Core.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ----- Services -----

// EF Core + SQL Server
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Customer API", Version = "v1" });
});

// CORS — allow Angular dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                      ?? new[] { "http://localhost:4200" };
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ----- Middleware -----

app.UseCors("AllowAngular");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply pending migrations automatically in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    db.Database.Migrate();
}


// ----- Helper: Map Entity -> DTO -----

static CustomerDto ToDto(Customer c) => new()
{
    Id = c.Id,
    FirstName = c.FirstName,
    LastName = c.LastName,
    Email = c.Email,
    Phone = c.Phone,
    Company = c.Company,
    Status = c.Status,
    CreatedAt = c.CreatedAt
};

// ----- Endpoints -----

var customers = app.MapGroup("/api/customers").WithTags("Customers");

// GET /api/customers
customers.MapGet("/", async (ICustomerRepository repo) =>
{
    var list = await repo.GetAllAsync();
    return Results.Ok(list.Select(ToDto));
})
.WithName("GetAllCustomers")
.Produces<IEnumerable<CustomerDto>>(200);

// GET /api/customers/{id}
customers.MapGet("/{id:int}", async (int id, ICustomerRepository repo) =>
{
    var customer = await repo.GetByIdAsync(id);
    return customer is not null ? Results.Ok(ToDto(customer)) : Results.NotFound();
})
.WithName("GetCustomerById")
.Produces<CustomerDto>(200)
.Produces(404);

// POST /api/customers
customers.MapPost("/", async (CreateCustomerDto dto, ICustomerRepository repo) =>
{
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true))
    {
        return Results.ValidationProblem(
            validationResults.ToDictionary(
                v => v.MemberNames.FirstOrDefault() ?? string.Empty,
                v => new[] { v.ErrorMessage ?? "Validation error" }));
    }

    var customer = new Customer
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        Phone = dto.Phone,
        Company = dto.Company,
        Status = dto.Status
    };

    var created = await repo.AddAsync(customer);
    return Results.Created($"/api/customers/{created.Id}", ToDto(created));
})
.WithName("CreateCustomer")
.Accepts<CreateCustomerDto>("application/json")
.Produces<CustomerDto>(201)
.ProducesValidationProblem();

// PUT /api/customers/{id}
customers.MapPut("/{id:int}", async (int id, UpdateCustomerDto dto, ICustomerRepository repo) =>
{
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true))
    {
        return Results.ValidationProblem(
            validationResults.ToDictionary(
                v => v.MemberNames.FirstOrDefault() ?? string.Empty,
                v => new[] { v.ErrorMessage ?? "Validation error" }));
    }

    var changes = new Customer
    {
        FirstName = dto.FirstName ?? string.Empty,
        LastName = dto.LastName ?? string.Empty,
        Email = dto.Email ?? string.Empty,
        Phone = dto.Phone ?? string.Empty,
        Company = dto.Company ?? string.Empty,
        Status = dto.Status ?? string.Empty
    };

    var updated = await repo.UpdateAsync(id, changes);
    return updated is not null ? Results.Ok(ToDto(updated)) : Results.NotFound();
})
.WithName("UpdateCustomer")
.Accepts<UpdateCustomerDto>("application/json")
.Produces<CustomerDto>(200)
.Produces(404)
.ProducesValidationProblem();

// DELETE /api/customers/{id}
customers.MapDelete("/{id:int}", async (int id, ICustomerRepository repo) =>
{
    var deleted = await repo.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteCustomer")
.Produces(204)
.Produces(404);

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
