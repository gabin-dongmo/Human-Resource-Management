using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Domain;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class HireEmployeeCommand : ICommand<Result<Guid, Error>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
}