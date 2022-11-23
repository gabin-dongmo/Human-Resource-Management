using Bogus;
using HRManagement.Modules.Personnel.Domain.Employee;
using Shouldly;
using Xunit;

namespace HRManagement.Modules.Personnel.Domain.UnitTests;

public class EmployeeDateOfBirthShould
{
    [Fact]
    public void Fail_OnCreation_IfDateInFuture()
    {
        var dateCreation = DateOfBirth.Create(new Faker().Date.FutureDateOnly());

        dateCreation.Error.ShouldNotBeNull();
    }    
}