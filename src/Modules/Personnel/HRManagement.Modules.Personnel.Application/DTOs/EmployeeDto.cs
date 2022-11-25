namespace HRManagement.Modules.Personnel.Application.DTOs;

public class EmployeeDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public DateOnly HireDate { get; set; }
}