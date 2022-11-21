using CSharpFunctionalExtensions;
using HRManagement.Modules.Personnel.Domain.Employee.BusinessRules;

namespace HRManagement.Modules.Personnel.Domain.Employee;

public class EmailAddress : ValueObject
{
    public string Email { get; }

    private EmailAddress(string email)
    {
        Email = email;
    }

    public static Result<EmailAddress, Error> Create(string email)
    {
        var rule = CheckRule(new ValidEmailAddressRule(email));
        if (rule.IsFailure)
            return rule.Value;
        return new EmailAddress(email);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Email;
    }
}