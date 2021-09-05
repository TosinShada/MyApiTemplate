using AutoMapper;
using MyApiTemplate.Domain.Common;
using MyApiTemplate.Domain.Entities;
using MyApiTemplate.Service.Contract;
using MyApiTemplate.Service.DTO.Response;
using MyApiTemplate.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApiTemplate.Service.Implementation
{
    public class Repository : IRepository
    {
        private readonly IDBFactory _dbFactory;
        private readonly IClientFactory _clientFactory;
        private readonly IMapper _mapper;

        public Repository(IDBFactory dbFactory,
            IClientFactory clientFactory,
            IMapper mapper)
        {
            _dbFactory = dbFactory;
            _clientFactory = clientFactory;
            _mapper = mapper;
        }

        public async Task<Response<IEnumerable<PersonQueryResponse>>> GetPersons()
        {
            var data = await _dbFactory.GetAllPersonAsync();
            var persons = _mapper.Map<IEnumerable<PersonQueryResponse>>(data);

            return new Response<IEnumerable<PersonQueryResponse>>(persons, message: "Successfully retrieved all persons.");
        }

        public async Task<Response<PersonQueryResponse>> GetPersonById(long id)
        {
            var person = await _dbFactory.GetByIdAsync(id);
            PersonQueryResponse response = new PersonQueryResponse();

            if (person != null)
            {
                response = _mapper.Map<PersonQueryResponse>(person);
            }
            else
            {
                throw new ApiException($"Record with id: {id} does not exist.");
            }
            return new Response<PersonQueryResponse>(response, message: $"Successfully retrieved person with id - {id}.");
        }

        public async Task<Response<List<Student>>> GetStudents()
        {
            var person = await _clientFactory.GetDataAsync<List<StudentQueryResponse>>($"/api/users");
            List<Student> response = new List<Student>();

            if (person != null)
            {
                response = _mapper.Map<List<Student>>(person);
            }
            else
            {
                throw new ApiException($"No student exist in the school.");
            }
            return new Response<List<Student>>(response, message: $"Successfully retrieved students information.");
        }
    }
}
