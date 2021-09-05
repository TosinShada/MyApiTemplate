using MyApiTemplate.Domain.Common;
using MyApiTemplate.Domain.Entities;
using MyApiTemplate.Service.DTO.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApiTemplate.Service.Contract
{
    public interface IRepository
    {
        Task<Response<IEnumerable<PersonQueryResponse>>> GetPersons();
        Task<Response<PersonQueryResponse>> GetPersonById(long id);
        Task<Response<List<Student>>> GetStudents();

    }
}
