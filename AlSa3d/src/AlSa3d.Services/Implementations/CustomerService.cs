using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlSa3d.Core.Entities;
using AlSa3d.Core.Interfaces;
using AlSa3d.Core.DTOs;

namespace AlSa3d.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Contact> _contactRepository;

        public CustomerService(
            IRepository<Customer> customerRepository,
            IRepository<Address> addressRepository,
            IRepository<Contact> contactRepository)
        {
            _customerRepository = customerRepository;
            _addressRepository = addressRepository;
            _contactRepository = contactRepository;
        }

        public async Task<Result<IEnumerable<Customer>>> GetAllCustomersAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync(
                    c => c.Addresses,
                    c => c.Contacts);
                
                return Result.Success(customers.Where(c => !c.IsDeleted).OrderBy(c => c.Name));
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Customer>>($"فشل في جلب العملاء: {ex.Message}");
            }
        }

        public async Task<Result<Customer>> GetCustomerByIdAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id,
                    c => c.Addresses,
                    c => c.Contacts);

                if (customer == null || customer.IsDeleted)
                    return Result.Failure<Customer>("العميل غير موجود");

                return Result.Success(customer);
            }
            catch (Exception ex)
            {
                return Result.Failure<Customer>($"فشل في جلب العميل: {ex.Message}");
            }
        }

        public async Task<Result<Customer>> CreateCustomerAsync(CreateCustomerDto dto)
        {
            try
            {
                // التحقق من عدم التكرار
                var existing = await _customerRepository.GetAsync(c => c.Phone == dto.Phone && !c.IsDeleted);
                if (existing != null)
                    return Result.Failure<Customer>("رقم الهاتف مسجل بالفعل");

                var customer = new Customer
                {
                    Name = dto.Name,
                    Type = dto.Type,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    TaxNumber = dto.TaxNumber,
                    NationalId = dto.NationalId,
                    Address = dto.Address,
                    City = dto.City,
                    Governorate = dto.Governorate,
                    PostalCode = dto.PostalCode,
                    Notes = dto.Notes,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var result = await _customerRepository.AddAsync(customer);
                if (!result.Success)
                    return result;

                // إضافة العناوين الإضافية
                if (dto.Addresses != null && dto.Addresses.Any())
                {
                    foreach (var addr in dto.Addresses)
                    {
                        customer.Addresses.Add(new Address
                        {
                            CustomerId = customer.Id,
                            AddressType = addr.AddressType,
                            Street = addr.Street,
                            City = addr.City,
                            Governorate = addr.Governorate,
                            PostalCode = addr.PostalCode,
                            Country = addr.Country ?? "مصر",
                            IsDefault = addr.IsDefault
                        });
                    }
                }

                // إضافة جهات الاتصال الإضافية
                if (dto.Contacts != null && dto.Contacts.Any())
                {
                    foreach (var contact in dto.Contacts)
                    {
                        customer.Contacts.Add(new Contact
                        {
                            CustomerId = customer.Id,
                            Name = contact.Name,
                            Title = contact.Title,
                            Phone = contact.Phone,
                            Email = contact.Email,
                            IsPrimary = contact.IsPrimary
                        });
                    }
                }

                await _customerRepository.UpdateAsync(customer);

                return Result.Success(customer);
            }
            catch (Exception ex)
            {
                return Result.Failure<Customer>($"فشل في إضافة العميل: {ex.Message}");
            }
        }

        public async Task<Result<Customer>> UpdateCustomerAsync(int id, UpdateCustomerDto dto)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id, c => c.Addresses, c => c.Contacts);
                if (customer == null || customer.IsDeleted)
                    return Result.Failure<Customer>("العميل غير موجود");

                // التحقق من عدم تكرار الهاتف
                if (dto.Phone != customer.Phone)
                {
                    var existing = await _customerRepository.GetAsync(c => c.Phone == dto.Phone && c.Id != id && !c.IsDeleted);
                    if (existing != null)
                        return Result.Failure<Customer>("رقم الهاتف مسجل لعميل آخر");
                }

                customer.Name = dto.Name;
                customer.Type = dto.Type;
                customer.Phone = dto.Phone;
                customer.Email = dto.Email;
                customer.TaxNumber = dto.TaxNumber;
                customer.NationalId = dto.NationalId;
                customer.Address = dto.Address;
                customer.City = dto.City;
                customer.Governorate = dto.Governorate;
                customer.PostalCode = dto.PostalCode;
                customer.Notes = dto.Notes;
                customer.IsActive = dto.IsActive ?? customer.IsActive;
                customer.UpdatedAt = DateTime.Now;

                var result = await _customerRepository.UpdateAsync(customer);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Customer>($"فشل في تحديث العميل: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteCustomerAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null || customer.IsDeleted)
                    return Result.Failure<bool>("العميل غير موجود");

                // التحقق من وجود فواتير
                // يمكن إضافة تحقق هنا

                customer.IsDeleted = true;
                customer.DeletedAt = DateTime.Now;

                var result = await _customerRepository.UpdateAsync(customer);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"فشل في حذف العميل: {ex.Message}");
            }
        }

        public async Task<Result<Address>> AddAddressAsync(int customerId, CreateAddressDto dto)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null || customer.IsDeleted)
                    return Result.Failure<Address>("العميل غير موجود");

                var address = new Address
                {
                    CustomerId = customerId,
                    AddressType = dto.AddressType,
                    Street = dto.Street,
                    City = dto.City,
                    Governorate = dto.Governorate,
                    PostalCode = dto.PostalCode,
                    Country = dto.Country ?? "مصر",
                    IsDefault = dto.IsDefault
                };

                // إذا كان العنوان افتراضي، نجعل باقي العناوين غير افتراضية
                if (dto.IsDefault)
                {
                    var addresses = await _addressRepository.GetAllAsync(a => a.CustomerId == customerId && !a.IsDeleted);
                    foreach (var addr in addresses)
                    {
                        addr.IsDefault = false;
                        await _addressRepository.UpdateAsync(addr);
                    }
                }

                var result = await _addressRepository.AddAsync(address);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Address>($"فشل في إضافة العنوان: {ex.Message}");
            }
        }

        public async Task<Result<Contact>> AddContactAsync(int customerId, CreateContactDto dto)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null || customer.IsDeleted)
                    return Result.Failure<Contact>("العميل غير موجود");

                var contact = new Contact
                {
                    CustomerId = customerId,
                    Name = dto.Name,
                    Title = dto.Title,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    IsPrimary = dto.IsPrimary
                };

                // إذا كان جهة الاتصال أساسية، نجعل الباقي غير أساسي
                if (dto.IsPrimary)
                {
                    var contacts = await _contactRepository.GetAllAsync(c => c.CustomerId == customerId && !c.IsDeleted);
                    foreach (var cont in contacts)
                    {
                        cont.IsPrimary = false;
                        await _contactRepository.UpdateAsync(cont);
                    }
                }

                var result = await _contactRepository.AddAsync(contact);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Contact>($"فشل في إضافة جهة الاتصال: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Customer>>> SearchCustomersAsync(string searchTerm)
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                
                var filtered = customers.Where(c => !c.IsDeleted && (
                    c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (c.Phone != null && c.Phone.Contains(searchTerm)) ||
                    (c.Email != null && c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.NationalId != null && c.NationalId.Contains(searchTerm))
                ));

                return Result.Success(filtered.OrderBy(c => c.Name));
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Customer>>($"فشل في البحث عن العملاء: {ex.Message}");
            }
        }

        public async Task<Result<decimal>> GetCustomerBalanceAsync(int customerId)
        {
            try
            {
                // سيتم تنفيذ هذا في InvoiceService
                return Result.Success(0m);
            }
            catch (Exception ex)
            {
                return Result.Failure<decimal>($"فشل في حساب رصيد العميل: {ex.Message}");
            }
        }

        public async Task<Result<CustomerStatisticsDto>> GetCustomerStatisticsAsync(int customerId)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null || customer.IsDeleted)
                    return Result.Failure<CustomerStatisticsDto>("العميل غير موجود");

                var stats = new CustomerStatisticsDto
                {
                    CustomerId = customerId,
                    CustomerName = customer.Name,
                    TotalInvoices = 0, // سيتم حسابه من InvoiceService
                    TotalPurchases = 0,
                    TotalReturns = 0,
                    CurrentBalance = 0,
                    LastPurchaseDate = null,
                    AverageMonthlyPurchases = 0
                };

                return Result.Success(stats);
            }
            catch (Exception ex)
            {
                return Result.Failure<CustomerStatisticsDto>($"فشل في جلب إحصائيات العميل: {ex.Message}");
            }
        }
    }
}
