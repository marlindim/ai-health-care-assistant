using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class MedicalRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public string Content { get; set; } = string.Empty;
        [NotMapped]
        public string? Summary { get; set; }
        public string EncryptedContent { get; set; } = string.Empty;
        public string? EncryptedSummary { get; set; }

    }
}