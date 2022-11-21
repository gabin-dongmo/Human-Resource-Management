namespace HRManagement.Modules.Personnel.Domain.Employee;

public class Employee : Entity<Guid>
{
    public Name Name { get; }
    public EmailAddress EmailAddress { get; }
    public DateOfBirth DateOfBirth { get; }
    public DateOnly HireDate { get; }
    public DateOnly? TerminationDate { get; set; }

    public Employee(Name name, EmailAddress emailAddress, DateOfBirth dateOfBirth)
    {
        Name = name;
        EmailAddress = emailAddress;
        DateOfBirth = dateOfBirth;
        HireDate = DateOnly.FromDateTime(DateTime.Now);
    }
}