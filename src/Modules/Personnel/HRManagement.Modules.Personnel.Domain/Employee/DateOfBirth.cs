using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Domain.Employee.BusinessRules;

namespace HRManagement.Modules.Personnel.Domain.Employee;

public class DateOfBirth : ValueObject
{
    public DateOnly Date { get; }

    private DateOfBirth(DateOnly date)
    {
        Date = date;
    }

    public static Result<DateOfBirth, Error> Create(string date)
    {
        var actualDateRule = CheckRule(new DateOfBirthMustBeActualDateRule(date));
        if (actualDateRule.IsFailure)
            return Error.Deserialize(actualDateRule.Error);

        var actualDate = DateOnly.FromDateTime(DateTime.Parse(date));
        
        var rule = CheckRule(new DateOfBirthNotInFutureRule(actualDate));
        if (rule.IsFailure)
            return Error.Deserialize(rule.Error);
        return new DateOfBirth(actualDate);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Date;
    }
}