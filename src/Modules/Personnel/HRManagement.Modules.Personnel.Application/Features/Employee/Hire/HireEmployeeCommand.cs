﻿using CSharpFunctionalExtensions;
using HRManagement.Common.Domain;
using HRManagement.Modules.Personnel.Application.Contracts.Handlers;
using HRManagement.Modules.Personnel.Application.DTOs;

namespace HRManagement.Modules.Personnel.Application.Features.Employee;

public class HireEmployeeCommand : ICommand<Result<EmployeeDto, List<Error>>>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
}