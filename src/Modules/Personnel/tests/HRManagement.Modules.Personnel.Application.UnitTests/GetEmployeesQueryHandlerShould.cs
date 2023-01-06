﻿using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using Bogus;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using HRManagement.Modules.Personnel.Domain.Employee;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Application.UnitTests;

public class GetEmployeesQueryHandlerShould
{
    [Fact]
    public async Task ReturnListOfEmployees_WhenCalled()
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var person = new Faker().Person;
        var employee = Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            DateOfBirth.Create(person.DateOfBirth.ToString("d")).Value).Value;
        mockEmployeeRepo
            .Setup(d => d.GetAsync(It.IsAny<Expression<Func<Employee, bool>>>(), It.IsAny<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>>(), It.IsNotNull<string>()))
            .ReturnsAsync(new List<Employee> {employee});
        var sut = fixture.Create<GetEmployeesQueryHandler>();

        var result = await sut.Handle(fixture.Create<GetEmployeesQuery>(), CancellationToken.None);

        result.Value.ShouldNotBeNull();
        result.Value.First().FirstName.ShouldBe(person.FirstName);
    }

    private static IFixture SetFixture(out Mock<IEmployeeRepository> mockEmployeeRepo)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        mockEmployeeRepo = fixture.Freeze<Mock<IEmployeeRepository>>();
        return fixture;
    }
}