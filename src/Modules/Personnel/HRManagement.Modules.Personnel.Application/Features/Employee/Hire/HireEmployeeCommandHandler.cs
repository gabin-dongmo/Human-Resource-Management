using System.Linq.Expressions;
using AutoMapper;
using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class HireEmployeeCommandHandler : ICommandHandler<HireEmployeeCommand, Result<EmployeeDto, Error>>
{
    private readonly IMapper _mapper;
    private readonly IEmployeeRepository _repository;

    public HireEmployeeCommandHandler(IMapper mapper, IEmployeeRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<EmployeeDto, Error>> Handle(HireEmployeeCommand request, CancellationToken cancellationToken)
    {
        var nameCreation = Name.Create(request.FirstName, request.LastName);
        if (nameCreation.IsFailure) return nameCreation.Error;

        var emailCreation = EmailAddress.Create(request.EmailAddress);
        if (emailCreation.IsFailure) return emailCreation.Error;

        var dateOfBirthCreation = DateOfBirth.Create(request.DateOfBirth);
        if (dateOfBirthCreation.IsFailure) return dateOfBirthCreation.Error;

        var actualDate = DateOnly.FromDateTime(DateTime.Parse(request.DateOfBirth));
        Expression<Func<Domain.Employee.Employee,bool>> existingEmployeeCondition = 
            e => e.Name.FirstName == request.FirstName 
                 && e.Name.LastName == request.LastName 
                 && e.DateOfBirth.Date == actualDate;
        
        var existingEmployees = await _repository.GetAsync(existingEmployeeCondition);
        if (existingEmployees.Any()) return DomainErrors.ResourceAlreadyExists();

        var employeeCreation = Domain.Employee.Employee.Create(nameCreation.Value, emailCreation.Value, dateOfBirthCreation.Value);
        if (employeeCreation.IsFailure) return employeeCreation.Error;

        var employee = employeeCreation.Value;
        await _repository.AddAsync(employee);
        await _repository.CommitAsync();

        return _mapper.Map<EmployeeDto>(employee);
    }
}