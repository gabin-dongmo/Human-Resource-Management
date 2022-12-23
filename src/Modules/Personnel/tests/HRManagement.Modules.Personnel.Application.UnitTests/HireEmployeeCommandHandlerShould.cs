using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using Bogus;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Application.UnitTests;

public class HireEmployeeCommandHandlerShould
{
    [Theory]
    [ClassData(typeof(InvalidNameOnCreationTestData))]
    public async Task ReturnError_WhenNameInvalid(HireEmployeeCommand hireEmployeeCommand)
    {
        var fixture = SetFixture(out _);
        var sut = fixture.Create<HireEmployeeCommandHandler>();

        var result = await sut.Handle(hireEmployeeCommand, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }

    [Theory]
    [ClassData(typeof(InvalidEmailOnCreationTestData))]
    public async Task ReturnError_WhenEmailInvalid(HireEmployeeCommand hireEmployeeCommand)
    {
        var fixture = SetFixture(out _);
        var sut = fixture.Create<HireEmployeeCommandHandler>();

        var result = await sut.Handle(hireEmployeeCommand, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }

    [Theory]
    [ClassData(typeof(InvalidDateOfBirthTestData))]
    public async Task ReturnError_WhenDateOfBirthInvalid(HireEmployeeCommand hireEmployeeCommand)
    {
        var fixture = SetFixture(out _);
        var sut = fixture.Create<HireEmployeeCommandHandler>();

        var result = await sut.Handle(hireEmployeeCommand, CancellationToken.None);
        
        result.Error.ShouldNotBeNull();
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeAlreadyExists()
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var person = new Faker().Person;
        mockEmployeeRepo
            .Setup(d => d.GetAsync(It.IsAny<Expression<Func<Employee,bool>>>(), It.IsAny<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>>()))
            .ReturnsAsync(new List<Employee> {BuildFakeEmployee(person)});
        var sut = fixture.Create<HireEmployeeCommandHandler>();

        var result = await sut.Handle(BuildFakeCommand(person), CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.ResourceAlreadyExists().Code);
    }

    [Fact]
    public async Task StoreNewEmployeeAndReturnEmployeeId_WhenHiringSuccessful()
    {
        var fixture = SetFixture(out var mockEmployeeRepo);
        var person = new Faker().Person;
        var employeesRepo = new List<Employee>();
        mockEmployeeRepo
            .Setup(d => d.GetAsync(It.IsAny<Expression<Func<Employee,bool>>>(), It.IsAny<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>>()))
            .ReturnsAsync(new List<Employee>());
        mockEmployeeRepo
            .Setup(d => d.CommitAsync())
            .Callback(() => employeesRepo.Add(BuildFakeEmployee(person)));
        var sut = fixture.Create<HireEmployeeCommandHandler>();

        var result = await sut.Handle(BuildFakeCommand(person), CancellationToken.None);

        result.Value.ShouldBe(Guid.Empty);
        employeesRepo.Count.ShouldBe(1);
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

    private static HireEmployeeCommand BuildFakeCommand(Person person)
    {
        var hireEmployee = new HireEmployeeCommand
        {
            EmailAddress = person.Email,
            FirstName = person.FirstName,
            LastName = person.LastName,
            DateOfBirth = person.DateOfBirth.ToString("d")
        };
        return hireEmployee;
    }
}

public class InvalidDateOfBirthTestData : TheoryData<HireEmployeeCommand>
{
    public InvalidDateOfBirthTestData()
    {
        var person = new Faker().Person;

        Add(new HireEmployeeCommand {EmailAddress = person.Email, FirstName = person.FirstName, LastName = person.LastName, DateOfBirth = null!});
        Add(new HireEmployeeCommand {EmailAddress = person.Email, FirstName = person.FirstName, LastName = person.LastName, DateOfBirth = string.Empty});
        Add(new HireEmployeeCommand {EmailAddress = person.Email, FirstName = person.FirstName, LastName = person.LastName, DateOfBirth = new Faker().Random.AlphaNumeric(9)});
    }
}

public class InvalidEmailOnCreationTestData : TheoryData<HireEmployeeCommand>
{
    public InvalidEmailOnCreationTestData()
    {
        var person = new Faker().Person;

        Add(new HireEmployeeCommand {EmailAddress = null!, FirstName = person.FirstName, LastName = person.LastName, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new HireEmployeeCommand {EmailAddress = string.Empty, FirstName = person.FirstName, LastName = person.LastName, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new HireEmployeeCommand {EmailAddress = new Faker().Random.AlphaNumeric(9), FirstName = person.FirstName, LastName = person.LastName, DateOfBirth = person.DateOfBirth.ToString("d")});
    }
}

public class InvalidNameOnCreationTestData : TheoryData<HireEmployeeCommand>
{
    public InvalidNameOnCreationTestData()
    {
        var person = new Faker().Person;
        
        Add(new HireEmployeeCommand {EmailAddress = person.Email, FirstName = null!, LastName = person.LastName, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new HireEmployeeCommand {EmailAddress = person.Email, FirstName = string.Empty, LastName = person.LastName, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new HireEmployeeCommand {EmailAddress = person.Email, FirstName = null!, LastName = new Faker().Random.AlphaNumeric(9), DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new HireEmployeeCommand {EmailAddress = person.Email, FirstName = string.Empty, LastName = new Faker().Random.AlphaNumeric(9), DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new HireEmployeeCommand {EmailAddress = person.Email, FirstName = person.FirstName, LastName = null!, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new HireEmployeeCommand {EmailAddress = person.Email, FirstName = person.FirstName, LastName = string.Empty, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new HireEmployeeCommand {EmailAddress = person.Email, FirstName = new Faker().Random.AlphaNumeric(9), LastName = null!, DateOfBirth = person.DateOfBirth.ToString("d")});
        Add(new HireEmployeeCommand {EmailAddress = person.Email, FirstName = new Faker().Random.AlphaNumeric(9), LastName = string.Empty, DateOfBirth = person.DateOfBirth.ToString("d")});
    }
}