using AutoMapper;
using MyApiTemplate.Domain.Entities;
using MyApiTemplate.Service.DTO.Request;
using MyApiTemplate.Service.DTO.Response;

namespace MyApiTemplate.Infrastructure.Configs
{
    public class MappingProfileConfiguration : Profile
    {
        public MappingProfileConfiguration()
        {
            CreateMap<Person, CreatePersonRequest>().ReverseMap();
            CreateMap<Person, UpdatePersonRequest>().ReverseMap();
            CreateMap<Person, PersonQueryResponse>().ReverseMap();
            CreateMap<Student, StudentQueryResponse>().ReverseMap();
        }
    }
}
