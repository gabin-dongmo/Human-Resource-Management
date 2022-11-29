using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Domain;
using MediatR;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class TerminateEmployee : ICommand<Result<Unit, Error>>
{
    public string EmployeeId { get; set; }
}