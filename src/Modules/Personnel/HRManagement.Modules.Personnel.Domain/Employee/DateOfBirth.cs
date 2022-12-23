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

    public static Result<DateOfBirth, List<Error>> Create(string date)
    {
        var errors = CheckForErrors(date, out var actualDate);
        if (errors.Any()) return errors;

        return new DateOfBirth(actualDate);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Date;
    }

    private static List<Error> CheckForErrors(string date, out DateOnly actualDate)
    {
        var actualDateRule = CheckRule(new DateOfBirthMustBeActualDateRule(date));
        if (actualDateRule.IsFailure)
            return new List<Error>{Error.Deserialize(actualDateRule.Error)};

        actualDate = DateOnly.FromDateTime(DateTime.Parse(date));
        
        var rule = CheckRule(new DateOfBirthNotInFutureRule(actualDate));
        return rule.IsFailure ? new List<Error>{Error.Deserialize(rule.Error)} : new List<Error>();
    }
}