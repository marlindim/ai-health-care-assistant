namespace Core.Dtos
{
    public class MedicalRecordDto
    {
        public record MedicalRecordUploadDto(Guid PatientId, string Content);
    }
}
