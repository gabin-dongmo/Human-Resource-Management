using CSharpFunctionalExtensions;
using HRManagement.Common.Domain.Models;
using HRManagement.Modules.Personnel.Domain.Employee.BusinessRules;
using ValueObject = HRManagement.Common.Domain.Models.ValueObject;

namespace HRManagement.Modules.Personnel.Domain.Employee;

public class EmailAddress : ValueObject
{
    protected EmailAddress()
    {
    }

    private EmailAddress(string email)
    {
        Email = email;
    }

    public string Email { get; } = null!;

    public static Result<EmailAddress, List<Error>> Create(string email)
    {
        var errors = ValidateBusinessRules(email);
        if (errors.Any()) return errors;

        return new EmailAddress(email);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Email;
    }

    private static List<Error> ValidateBusinessRules(string email)
    {
        var errors = new List<Error>();
        var notNullOrEmptyRule = CheckRule(new NotNullOrEmptyEmailAddressRule(email, nameof(Email)));
        if (notNullOrEmptyRule.IsFailure)
            errors.Add(Error.Deserialize(notNullOrEmptyRule.Error));

        var validEmailRule = CheckRule(new ValidEmailAddressRule(email));
        if (validEmailRule.IsFailure)
            errors.Add(Error.Deserialize(validEmailRule.Error));
        return errors;
    }
}