using CustomerApi.Core.Data;
using CustomerApi.Core.DTOs;
using CustomerApi.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:int}/addresses")]
public class AddressesController : ControllerBase
{
    private readonly CustomerDbContext _context;

    public AddressesController(CustomerDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int customerId)
    {
        var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);
        if (!customerExists) return NotFound();

        var addresses = await _context.CustomerAddresses
            .Where(a => a.CustomerId == customerId)
            .AsNoTracking()
            .Select(a => ToDto(a))
            .ToListAsync();

        return Ok(addresses);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int customerId, int id)
    {
        var address = await _context.CustomerAddresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.CustomerId == customerId);

        return address is not null ? Ok(ToDto(address)) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int customerId, [FromBody] CreateAddressDto dto)
    {
        var customerExists = await _context.Customers.AnyAsync(c => c.Id == customerId);
        if (!customerExists) return NotFound();

        var address = new CustomerAddress
        {
            CustomerId = customerId,
            AddressType = dto.AddressType,
            AddressLine1 = dto.AddressLine1,
            AddressLine2 = dto.AddressLine2,
            City = dto.City,
            StateProvince = dto.StateProvince,
            PostalCode = dto.PostalCode,
            CountryCode = dto.CountryCode,
            IsPrimary = dto.IsPrimary
        };

        // If this is set as primary, unset other primaries
        if (dto.IsPrimary)
        {
            await _context.CustomerAddresses
                .Where(a => a.CustomerId == customerId && a.IsPrimary)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsPrimary, false));
        }

        _context.CustomerAddresses.Add(address);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { customerId, id = address.Id }, ToDto(address));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int customerId, int id, [FromBody] UpdateAddressDto dto)
    {
        var address = await _context.CustomerAddresses
            .FirstOrDefaultAsync(a => a.Id == id && a.CustomerId == customerId);

        if (address is null) return NotFound();

        if (dto.AddressType is not null) address.AddressType = dto.AddressType;
        if (dto.AddressLine1 is not null) address.AddressLine1 = dto.AddressLine1;
        if (dto.AddressLine2 is not null) address.AddressLine2 = dto.AddressLine2;
        if (dto.City is not null) address.City = dto.City;
        if (dto.StateProvince is not null) address.StateProvince = dto.StateProvince;
        if (dto.PostalCode is not null) address.PostalCode = dto.PostalCode;
        if (dto.CountryCode is not null) address.CountryCode = dto.CountryCode;
        if (dto.IsPrimary.HasValue)
        {
            if (dto.IsPrimary.Value)
            {
                await _context.CustomerAddresses
                    .Where(a => a.CustomerId == customerId && a.Id != id && a.IsPrimary)
                    .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsPrimary, false));
            }
            address.IsPrimary = dto.IsPrimary.Value;
        }

        await _context.SaveChangesAsync();
        return Ok(ToDto(address));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int customerId, int id)
    {
        var address = await _context.CustomerAddresses
            .FirstOrDefaultAsync(a => a.Id == id && a.CustomerId == customerId);

        if (address is null) return NotFound();

        _context.CustomerAddresses.Remove(address);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private static CustomerAddressDto ToDto(CustomerAddress a) => new()
    {
        Id = a.Id,
        CustomerId = a.CustomerId,
        AddressType = a.AddressType,
        AddressLine1 = a.AddressLine1,
        AddressLine2 = a.AddressLine2,
        City = a.City,
        StateProvince = a.StateProvince,
        PostalCode = a.PostalCode,
        CountryCode = a.CountryCode,
        IsPrimary = a.IsPrimary
    };
}
