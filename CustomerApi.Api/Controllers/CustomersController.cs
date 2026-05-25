using CustomerApi.Core.DTOs;
using CustomerApi.Core.Entities;
using CustomerApi.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _repo;

    public CustomersController(ICustomerRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var list = await _repo.GetAllAsync();
        return Ok(list.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CustomerDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _repo.GetByIdAsync(id);
        return customer is not null ? Ok(ToDto(customer)) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Status = dto.Status,
            CustomerType = dto.CustomerType
        };

        var created = await _repo.AddAsync(customer);

        // Create business profile if company name provided
        if (!string.IsNullOrWhiteSpace(dto.CompanyName))
        {
            created.BusinessProfile = new CustomerBusinessProfile
            {
                CustomerId = created.Id,
                CompanyName = dto.CompanyName,
                LifecycleStage = "Lead"
            };
        }

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CustomerDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
    {
        var changes = new Customer
        {
            FirstName = dto.FirstName ?? string.Empty,
            LastName = dto.LastName ?? string.Empty,
            Email = dto.Email ?? string.Empty,
            Phone = dto.Phone ?? string.Empty,
            Status = dto.Status ?? string.Empty,
            CustomerType = dto.CustomerType ?? string.Empty
        };

        var updated = await _repo.UpdateAsync(id, changes);
        if (updated is null) return NotFound();

        // Update business profile if CompanyName provided
        if (dto.CompanyName is not null)
        {
            var ctx = HttpContext.RequestServices.GetRequiredService<CustomerApi.Core.Data.CustomerDbContext>();
            var profile = await ctx.CustomerBusinessProfiles.FirstOrDefaultAsync(p => p.CustomerId == id);
            if (profile is not null)
            {
                profile.CompanyName = dto.CompanyName;
            }
            else
            {
                ctx.CustomerBusinessProfiles.Add(new CustomerBusinessProfile
                {
                    CustomerId = id,
                    CompanyName = dto.CompanyName,
                    LifecycleStage = "Customer"
                });
            }
            await ctx.SaveChangesAsync();
            updated.BusinessProfile = await ctx.CustomerBusinessProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.CustomerId == id);
        }

        return Ok(ToDto(updated));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    private static CustomerDto ToDto(Customer c) => new()
    {
        Id = c.Id,
        FirstName = c.FirstName,
        LastName = c.LastName,
        Email = c.Email,
        Phone = c.Phone,
        Status = c.Status,
        CustomerType = c.CustomerType,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt,
        CompanyName = c.BusinessProfile?.CompanyName
    };
}
