using AlSa3d.Core;
using AlSa3d.Core.DTOs;
using AlSa3d.Core.Entities;

namespace AlSa3d.Services.Interfaces;

public interface ICustomerService
{
    Task<Result<IEnumerable<Customer>>> GetAllCustomersAsync();
    Task<Result<Customer>> GetCustomerByIdAsync(int id);
    Task<Result<Customer>> CreateCustomerAsync(CreateCustomerDto dto);
    Task<Result<Customer>> UpdateCustomerAsync(int id, UpdateCustomerDto dto);
    Task<Result<bool>> DeleteCustomerAsync(int id);
    Task<Result<IEnumerable<Customer>>> SearchCustomersAsync(string searchTerm);
    Task<Result<decimal>> GetCustomerBalanceAsync(int customerId);
    Task<Result<CustomerStatisticsDto>> GetCustomerStatisticsAsync(int customerId);
    Task<Result<Address>> AddAddressAsync(int customerId, CreateAddressDto dto);
    Task<Result<Contact>> AddContactAsync(int customerId, CreateContactDto dto);
}
