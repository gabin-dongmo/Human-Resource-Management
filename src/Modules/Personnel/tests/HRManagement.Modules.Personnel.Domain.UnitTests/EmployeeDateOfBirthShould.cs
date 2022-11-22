using Bogus;
using HRManagement.Modules.Personnel.Domain.Employee;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Domain.UnitTests;

public class EmployeeDateOfBirthShould
{
    [Fact]
    public void NotBeInTheFuture()
    {
        var dateCreation = DateOfBirth.Create(new Faker().Date.FutureDateOnly());

        dateCreation.Error.ShouldNotBeNull();
    }    
}