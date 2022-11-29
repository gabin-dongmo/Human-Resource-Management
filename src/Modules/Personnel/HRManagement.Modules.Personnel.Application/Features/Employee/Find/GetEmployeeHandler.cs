using AutoMapper;
using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class GetEmployeeHandler : IQueryHandler<GetEmployee, Result<EmployeeDto, Error>>
{
    private readonly IMapper _mapper;
    private readonly IEmployeeRepository _repository;

    public GetEmployeeHandler(IMapper mapper, IEmployeeRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<EmployeeDto, Error>> Handle(GetEmployee request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.EmployeeId, out var employeeId)) 
            return DomainErrors.NotFound(nameof(Domain.Employee.Employee), request.EmployeeId);

        Maybe<Domain.Employee.Employee> employee = await _repository.GetByIdAsync(employeeId);
        if (employee.HasNoValue)
            return DomainErrors.NotFound(nameof(Domain.Employee.Employee), request.EmployeeId);

        return _mapper.Map<EmployeeDto>(employee.Value);
    }
}