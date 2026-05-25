using CustomerApi.Core.Data;
using CustomerApi.Core.DTOs;
using CustomerApi.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:int}/profile")]
public class BusinessProfileController : ControllerBase
{
    private readonly CustomerDbContext _context;

    public BusinessProfileController(CustomerDbContext context)
    {
        _context = context;
    }

    // GET /api/customers/{customerId}/profile
    [HttpGet]
    public async Task<IActionResult> Get(int customerId)
    {
        var profile = await _context.CustomerBusinessProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.CustomerId == customerId);

        if (profile is null)
        {
            // Check customer exists
            var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);
            if (!customerExists) return NotFound();
            // Return empty profile
            return Ok(new BusinessProfileDto { CustomerId = customerId, LifecycleStage = "Lead" });
        }

        return Ok(ToDto(profile));
    }

    // PUT /api/customers/{customerId}/profile - create or update
    [HttpPut]
    public async Task<IActionResult> CreateOrUpdate(int customerId, [FromBody] CreateOrUpdateBusinessProfileDto dto)
    {
        var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);
        if (!customerExists) return NotFound();

        var profile = await _context.CustomerBusinessProfiles
            .FirstOrDefaultAsync(p => p.CustomerId == customerId);

        if (profile is null)
        {
            profile = new CustomerBusinessProfile
            {
                CustomerId = customerId,
                CompanyName = dto.CompanyName,
                JobTitle = dto.JobTitle,
                LeadSource = dto.LeadSource,
                LifecycleStage = dto.LifecycleStage ?? "Lead"
            };
            _context.CustomerBusinessProfiles.Add(profile);
        }
        else
        {
            if (dto.CompanyName is not null) profile.CompanyName = dto.CompanyName;
            if (dto.JobTitle is not null) profile.JobTitle = dto.JobTitle;
            if (dto.LeadSource is not null) profile.LeadSource = dto.LeadSource;
            if (dto.LifecycleStage is not null) profile.LifecycleStage = dto.LifecycleStage;
        }

        await _context.SaveChangesAsync();
        return Ok(ToDto(profile));
    }

    // DELETE /api/customers/{customerId}/profile
    [HttpDelete]
    public async Task<IActionResult> Delete(int customerId)
    {
        var profile = await _context.CustomerBusinessProfiles
            .FirstOrDefaultAsync(p => p.CustomerId == customerId);

        if (profile is null) return NotFound();

        _context.CustomerBusinessProfiles.Remove(profile);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static BusinessProfileDto ToDto(CustomerBusinessProfile p) => new()
    {
        Id = p.Id,
        CustomerId = p.CustomerId,
        CompanyName = p.CompanyName,
        JobTitle = p.JobTitle,
        LeadSource = p.LeadSource,
        LifecycleStage = p.LifecycleStage
    };
}
