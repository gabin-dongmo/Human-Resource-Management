using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;
using MediatR;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class UpdateEmployeeHandler : ICommandHandler<UpdateEmployee, Result<Unit, Error>>
{
    private readonly IEmployeeRepository _repository;

    public UpdateEmployeeHandler(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Unit, Error>> Handle(UpdateEmployee request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.Id, out var employeeId)) 
            return DomainErrors.NotFound(nameof(Domain.Employee.Employee), request.Id);

        Maybe<Domain.Employee.Employee> employeeOrNot = await _repository.GetByIdAsync(employeeId);
        if (employeeOrNot.HasNoValue) return DomainErrors.NotFound(nameof(Domain.Employee.Employee), employeeId);

        var nameCreation = Name.Create(request.FirstName, request.LastName);
        if (nameCreation.IsFailure) return nameCreation.Error;

        var emailCreation = EmailAddress.Create(request.EmailAddress);
        if (emailCreation.IsFailure) return emailCreation.Error;

        var dateOfBirthCreation = DateOfBirth.Create(request.DateOfBirth);
        if (dateOfBirthCreation.IsFailure) return dateOfBirthCreation.Error;

        employeeOrNot.Value.Update(nameCreation.Value, emailCreation.Value, dateOfBirthCreation.Value);
        await _repository.CommitAsync();

        return Unit.Value;
    }
}