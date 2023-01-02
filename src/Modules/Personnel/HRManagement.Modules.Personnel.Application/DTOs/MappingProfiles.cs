using AutoMapper;
using HRManagement.Common.Domain.Utilities;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using HRManagement.Modules.Personnel.Domain.Employee;

namespace HRManagement.Modules.Personnel.Application.DTOs;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.LastName))
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress.Email))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.Date.ToISO8601String()))
            .ForMember(dest => dest.HireDate, opt => opt.MapFrom(src => src.HireDate.ToISO8601String()));

        CreateMap<HireEmployeeDto, HireEmployeeCommand>();

        CreateMap<UpdateEmployeeDto, UpdateEmployeeCommand>();
    }
}