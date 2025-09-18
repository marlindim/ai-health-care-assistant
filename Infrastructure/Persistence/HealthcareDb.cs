using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class HealthcareDb(DbContextOptions db):DbContext(db)
    {
        public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
    }
}
