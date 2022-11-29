﻿using AutoFixture;
using AutoFixture.AutoMoq;
using Bogus;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Application.UnitTests;

public class UpdateEmployeeHandlerShould
{
    [Theory]
    [ClassData(typeof(InvalidNameOnUpdateTestData))]
    public async Task ReturnError_WhenNameInvalid(UpdateEmployee updateEmployee)
    {
        var fixture = SetFixture(out _);
        var sut = fixture.Create<UpdateEmployeeHandler>();

        var result = await sut.Handle(updateEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }

    [Theory]
    [ClassData(typeof(InvalidEmailOnUpdateTestData))]
    public async Task ReturnError_WhenEmailInvalid(UpdateEmployee updateEmployee)
    {
        var fixture = SetFixture(out _);
        var sut = fixture.Create<UpdateEmployeeHandler>();

        var result = await sut.Handle(updateEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }

    [Theory]
    [ClassData(typeof(InvalidDateOfBirthOnUpdateTestData))]
    public async Task ReturnError_WhenDateOfBirthInvalid(UpdateEmployee updateEmployee)
    {
        var fixture = SetFixture(out _);
        var sut = fixture.Create<UpdateEmployeeHandler>();

        var result = await sut.Handle(updateEmployee, CancellationToken.None);
        
        result.Error.ShouldNotBeNull();
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeNotFound()
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var person = new Faker().Person;
        var updateEmployee = BuildFakeCommand(person);
        mockEmployeeRepo
            .Setup(d => d.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => null!);
        var sut = fixture.Create<UpdateEmployeeHandler>();

        var result = await sut.Handle(updateEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.NotFound(nameof(Employee), updateEmployee.EmployeeId).Code);
    }
    
    [Fact]
    public async Task UpdateEmployee_WhenEmployeeExists()
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var person = new Faker().Person;
        var employee = BuildFakeEmployee(person);
        var updateEmployee = BuildFakeCommand(person);
        mockEmployeeRepo
            .Setup(d => d.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => employee);
        mockEmployeeRepo
            .Setup(d => d.CommitAsync())
            .Callback(() =>
            {
                var name = Name.Create($"{updateEmployee.FirstName} Updated", updateEmployee.LastName).Value;
                var email = EmailAddress.Create(updateEmployee.EmailAddress).Value;
                var dateOfBirth = DateOfBirth.Create(updateEmployee.DateOfBirth).Value;
                employee.Update(name, email, dateOfBirth);
            });
        var sut = fixture.Create<UpdateEmployeeHandler>();

        var result = await sut.Handle(updateEmployee, CancellationToken.None);

        result.Value.ShouldBe(Unit.Value);
        employee.Name.FirstName.ShouldEndWith(" Updated");
    }

    private static IFixture SetFixture(out Mock<IEmployeeRepository> mockEmployeeRepo)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        mockEmployeeRepo = fixture.Freeze<Mock<IEmployeeRepository>>();
        return fixture;
    }
    
    private static Employee BuildFakeEmployee(Person person)
    {
        var employee = Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            DateOfBirth.Create(person.DateOfBirth.ToString("d")).Value).Value;
        return employee;
    }

    private static UpdateEmployee BuildFakeCommand(Person person)
    {
        var hireEmployee = new UpdateEmployee
        {
            EmployeeId = Guid.NewGuid().ToString(),
            EmailAddress = person.Email,
            FirstName = person.FirstName,
            LastName = person.LastName,
            DateOfBirth = person.DateOfBirth.ToString("d")
        };
        return hireEmployee;
    }
}

public class InvalidNameOnUpdateTestData : TheoryData<UpdateEmployee>
{
    public InvalidNameOnUpdateTestData()
    {
        var person = new Faker().Person;
        
        Add(new UpdateEmployee {EmailAddress = person.Email, FirstName = null!, LastName = person.LastName, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new UpdateEmployee {EmailAddress = person.Email, FirstName = string.Empty, LastName = person.LastName, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new UpdateEmployee {EmailAddress = person.Email, FirstName = null!, LastName = new Faker().Random.AlphaNumeric(9), DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new UpdateEmployee {EmailAddress = person.Email, FirstName = string.Empty, LastName = new Faker().Random.AlphaNumeric(9), DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new UpdateEmployee {EmailAddress = person.Email, FirstName = person.FirstName, LastName = null!, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new UpdateEmployee {EmailAddress = person.Email, FirstName = person.FirstName, LastName = string.Empty, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new UpdateEmployee {EmailAddress = person.Email, FirstName = new Faker().Random.AlphaNumeric(9), LastName = null!, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new UpdateEmployee {EmailAddress = person.Email, FirstName = new Faker().Random.AlphaNumeric(9), LastName = string.Empty, DateOfBirth = person.DateOfBirth.ToString("d")});
    }
}

public class InvalidEmailOnUpdateTestData : TheoryData<UpdateEmployee>
{
    public InvalidEmailOnUpdateTestData()
    {
        var person = new Faker().Person;

        Add(new UpdateEmployee {EmailAddress = null!, FirstName = person.FirstName, LastName = person.LastName, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new UpdateEmployee {EmailAddress = string.Empty, FirstName = person.FirstName, LastName = person.LastName, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new UpdateEmployee {EmailAddress = new Faker().Random.AlphaNumeric(9), FirstName = person.FirstName, LastName = person.LastName, DateOfBirth = person.DateOfBirth.ToString("d")});
    }
}

public class InvalidDateOfBirthOnUpdateTestData : TheoryData<UpdateEmployee>
{
    public InvalidDateOfBirthOnUpdateTestData()
    {
        var person = new Faker().Person;

        Add(new UpdateEmployee {EmailAddress = person.Email, FirstName = person.FirstName, LastName = person.LastName, DateOfBirth = null!});
        Add(new UpdateEmployee {EmailAddress = person.Email, FirstName = person.FirstName, LastName = person.LastName, DateOfBirth = string.Empty});
        Add(new UpdateEmployee {EmailAddress = person.Email, FirstName = person.FirstName, LastName = person.LastName, DateOfBirth = new Faker().Random.AlphaNumeric(9)});
    }
}

