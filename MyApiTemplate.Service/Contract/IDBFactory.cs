using MyApiTemplate.Domain.Common;
using MyApiTemplate.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApiTemplate.Service.Contract
{
    public interface IDBFactory
    {
        Task<IEnumerable<Person>> GetAllPersonAsync();
        Task<Person> GetByIdAsync(object id);
        Task<long> CreateAsync(Person entity);
        Task<bool> UpdateAsync(Person entity);
        Task<bool> DeleteAsync(object id);
        Task<bool> ExistAsync(object id);
    }
}
