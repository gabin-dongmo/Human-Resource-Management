﻿using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Bogus;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.DTOs;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Application.UnitTests;

public class GetEmployeeHandlerShould
{
    [Fact]
    public async Task ReturnEmployee_WhenEmployeeExists()
    {
        var fixture = SetFixture(out var mockEmployeeRepo, out var mockMapper);
        var person = new Faker().Person;
        var employee = Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            DateOfBirth.Create(DateOnly.FromDateTime(person.DateOfBirth)).Value).Value;
        var employeeDto = new EmployeeDto
        {
            FirstName = person.FirstName,
            LastName = person.LastName,
            DateOfBirth = DateOnly.FromDateTime(person.DateOfBirth),
            EmailAddress = person.Email,
            HireDate = employee.HireDate
        };
        mockEmployeeRepo.Setup(d => d.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(employee);
        mockMapper.Setup(x => x.Map<EmployeeDto>(It.IsAny<Employee>())).Returns(employeeDto);
        var sut = fixture.Create<GetEmployeeHandler>();

        var result = await sut.Handle(fixture.Create<GetEmployee>(), CancellationToken.None);

        result.Value.ShouldNotBeNull();
        result.Value.FirstName.ShouldBe(person.FirstName);
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeDoesNotExist()
    {
        var fixture = SetFixture(out var mock, out _);
        mock.Setup(d => d.GetByIdAsync(It.IsAny<Guid>()))!.ReturnsAsync(default(Employee));
        var sut = fixture.Create<GetEmployeeHandler>();

        var result = await sut.Handle(fixture.Create<GetEmployee>(), CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.NotFound(It.IsAny<string>(), It.IsAny<Guid>()).Code);
    }

    private static IFixture SetFixture(out Mock<IEmployeeRepository> mockEmployeeRepo, out Mock<IMapper> mockMapper)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        mockEmployeeRepo = fixture.Freeze<Mock<IEmployeeRepository>>();
        mockMapper = fixture.Freeze<Mock<IMapper>>();
        return fixture;
    }
}