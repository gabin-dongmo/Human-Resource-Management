using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Domain.Employee.BusinessRules;

namespace HRManagement.Modules.Personnel.Domain.Employee;

public class Name : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }

    private Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<Name, Error> Create(string firstName, string lastName)
    {
        var firstNameRule = CheckRule(new ValidNameRule(firstName));
        if (firstNameRule.IsFailure)
            return firstNameRule.Value;
        
        var lastNameRule = CheckRule(new ValidNameRule(lastName));
        if (lastNameRule.IsFailure)
            return lastNameRule.Value;
        return new Name(firstName, lastName);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
}