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
            .Include(c => c.BusinessProfile)
            .OrderBy(c => c.Id)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers
            .Include(c => c.BusinessProfile)
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
        var existing = await _context.Customers
            .Include(c => c.BusinessProfile)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (existing == null) return null;

        if (!string.IsNullOrEmpty(changes.FirstName)) existing.FirstName = changes.FirstName;
        if (!string.IsNullOrEmpty(changes.LastName)) existing.LastName = changes.LastName;
        if (!string.IsNullOrEmpty(changes.Email)) existing.Email = changes.Email;
        if (!string.IsNullOrEmpty(changes.Phone)) existing.Phone = changes.Phone;
        if (!string.IsNullOrEmpty(changes.Status)) existing.Status = changes.Status;
        if (!string.IsNullOrEmpty(changes.CustomerType)) existing.CustomerType = changes.CustomerType;
        existing.UpdatedAt = DateTime.UtcNow;

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
