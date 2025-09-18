using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Patient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required] public string FullName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string? Email { get; set; }
        public ICollection<MedicalRecord> Records { get; set; } = [];
    
    }
}
