namespace Core.Dtos
{
    public class MedicalRecordSumDto
    {
        public record MedicalRecordSummaryDto(Guid RecordId, string Summary);
    }
}
