using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Bogus;
using HRManagement.Modules.Personnel.Application.Contracts;
using HRManagement.Modules.Personnel.Application.Features.Employee;
using HRManagement.Modules.Personnel.Domain;
using HRManagement.Modules.Personnel.Domain.Employee;
using Moq;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Application.UnitTests;

public class HireEmployeeHandlerShould
{
    [Theory]
    [ClassData(typeof(InvalidNameTestData))]
    public async Task ReturnError_WhenNameInvalid(string firstName, string lastName)
    {
        var fixture = SetFixture(out _, out _);
        var sut = fixture.Create<HireEmployeeHandler>();
        var person = new Faker().Person;
        var hireEmployee = new HireEmployee
        {
            EmailAddress = person.Email,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = person.DateOfBirth.ToString("d")
        };

        var result = await sut.Handle(hireEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }

    [Theory]
    [ClassData(typeof(InvalidEmailTestData))]
    public async Task ReturnError_WhenEmailInvalid(string email)
    {
        var fixture = SetFixture(out _, out _);
        var sut = fixture.Create<HireEmployeeHandler>();
        var person = new Faker().Person;
        var hireEmployee = new HireEmployee
        {
            EmailAddress = email,
            FirstName = person.FirstName,
            LastName = person.LastName,
            DateOfBirth = person.DateOfBirth.ToString("d")
        };

        var result = await sut.Handle(hireEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }
    
    [Theory]
    [ClassData(typeof(InvalidDateOfBirthTestData))]
    public async Task ReturnError_WhenDateOfBirthInvalid(string dateOfBirth)
    {
        var fixture = SetFixture(out _, out _);
        var sut = fixture.Create<HireEmployeeHandler>();
        var person = new Faker().Person;
        var hireEmployee = new HireEmployee
        {
            EmailAddress = person.Email,
            FirstName = person.FirstName,
            LastName = person.LastName,
            DateOfBirth = dateOfBirth
        };

        var result = await sut.Handle(hireEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
    }

    [Fact]
    public async Task ReturnError_WhenEmployeeAlreadyExists()
    {
        var fixture = SetFixture(out var mockEmployeeRepo, out _);
        var person = new Faker().Person;
        var employee = Employee.Create(
            Name.Create(person.FirstName, person.LastName).Value,
            EmailAddress.Create(person.Email).Value,
            DateOfBirth.Create(person.DateOfBirth.ToString("d")).Value).Value;
        var hireEmployee = fixture.Create<HireEmployee>();
        hireEmployee.FirstName = person.FirstName;
        hireEmployee.LastName = person.LastName;
        hireEmployee.DateOfBirth = person.DateOfBirth.ToString("d");
        hireEmployee.EmailAddress = person.Email;
        mockEmployeeRepo
            .Setup(d => d.GetAsync(It.IsAny<Expression<Func<Employee,bool>>>(), It.IsAny<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>>()))
            .ReturnsAsync(new List<Employee>{employee});
        var sut = fixture.Create<HireEmployeeHandler>();

        var result = await sut.Handle(hireEmployee, CancellationToken.None);

        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(DomainErrors.ResourceAlreadyExists().Code);
    }

    [Fact]
    public async Task ReturnEmployeeId_WhenHiringSuccessful()
    {
        var fixture = SetFixture(out var mockEmployeeRepo, out _);
        var person = new Faker().Person;
        var hireEmployee = fixture.Create<HireEmployee>();
        hireEmployee.FirstName = person.FirstName;
        hireEmployee.LastName = person.LastName;
        hireEmployee.DateOfBirth = person.DateOfBirth.ToString("d");
        hireEmployee.EmailAddress = person.Email;
        mockEmployeeRepo
            .Setup(d => d.GetAsync(It.IsAny<Expression<Func<Employee,bool>>>(), It.IsAny<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>>()))
            .ReturnsAsync(new List<Employee>());
        var sut = fixture.Create<HireEmployeeHandler>();

        var result = await sut.Handle(hireEmployee, CancellationToken.None);

        result.Value.ShouldBe(Guid.Empty);
    }
    
    private static IFixture SetFixture(out Mock<IEmployeeRepository> mockEmployeeRepo, out Mock<IMapper> mockMapper)
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        mockEmployeeRepo = fixture.Freeze<Mock<IEmployeeRepository>>();
        mockMapper = fixture.Freeze<Mock<IMapper>>();
        return fixture;
    }
}

public class InvalidDateOfBirthTestData : TheoryData<string>
{
    public InvalidDateOfBirthTestData()
    {
        Add(null!);
        Add(string.Empty);
        Add(new Faker().Random.AlphaNumeric(9));
    }
}

public class InvalidEmailTestData : TheoryData<string>
{
    public InvalidEmailTestData()
    {
        Add(null!);
        Add(string.Empty);
        Add(new Faker().Random.AlphaNumeric(9));
    }
}

public class InvalidNameTestData : TheoryData<string, string>
{
    public InvalidNameTestData()
    {
        var person = new Faker().Person;
        
        Add(null!, person.LastName);
        Add(string.Empty, person.LastName);
        Add(null!, new Faker().Random.AlphaNumeric(9));
        Add(string.Empty, new Faker().Random.AlphaNumeric(9));
        Add(person.FirstName, null!);
        Add(person.FirstName, string.Empty);
        Add(new Faker().Random.AlphaNumeric(9), null!);
        Add(new Faker().Random.AlphaNumeric(9), string.Empty);
    }
}