using CustomerApi.Core.Entities;

namespace CustomerApi.Core.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer> AddAsync(Customer customer);
    Task<Customer?> UpdateAsync(int id, Customer changes);
    Task<bool> DeleteAsync(int id);
}
