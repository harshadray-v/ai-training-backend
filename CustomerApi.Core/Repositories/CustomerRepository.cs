using CustomerApi.Core.Data;
using CustomerApi.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Core.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomerDbContext _context;

    public CustomerRepository(CustomerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .OrderBy(c => c.Id)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        customer.CreatedAt = DateTime.UtcNow;
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer?> UpdateAsync(int id, Customer changes)
    {
        var existing = await _context.Customers.FindAsync(id);
        if (existing == null) return null;

        if (changes.FirstName is not null) existing.FirstName = changes.FirstName;
        if (changes.LastName is not null) existing.LastName = changes.LastName;
        if (changes.Email is not null) existing.Email = changes.Email;
        if (changes.Phone is not null) existing.Phone = changes.Phone;
        if (changes.Company is not null) existing.Company = changes.Company;
        if (changes.Status is not null) existing.Status = changes.Status;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }
}
