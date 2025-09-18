namespace Core.Dtos
{
    //internal class PatientDto
    //{
    //}
    // For returning patient info
    public record PatientDto(
        Guid Id,
        string FullName,
        DateOnly DateOfBirth,
       // int Age,
        string? Email
    );

    // For creating a new patient
    public record CreatePatientDto(
        string FullName,
        DateOnly DateOfBirth,
        string? Email
    );

    // For updating an existing patient
    public record UpdatePatientDto(
        string FullName,
        DateOnly DateOfBirth,
        string? Email
    );
}
