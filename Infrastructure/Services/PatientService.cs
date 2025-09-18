using Core.Contract;
using Core.Dtos;
using Core.Entities;

namespace Infrastructure.Services
{
    public class PatientService : IPatientService
    {
        private readonly IGenericService<Patient> _patient;

        public PatientService(IGenericService<Patient> patient)
        {
            _patient = patient;
        }

        public async Task<IEnumerable<PatientDto>> GetAllAsync()
        {
            var patients = await _patient.GetAllAsync();
            return patients.Select(p => new PatientDto(
                p.Id,
                p.FullName,
                p.DateOfBirth,
                p.Email
            ));
        }

        public async Task<PatientDto?> GetByIdAsync(Guid id)
        {
            var patient = await _patient.GetAsync(id);
            if (patient is null) return null;

            return new PatientDto(
                patient.Id,
                patient.FullName,
                patient.DateOfBirth,
                patient.Email
            );
        }

        public async Task<PatientDto> CreateAsync(CreatePatientDto dto)
        {
            var entity = new Patient
            {
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email
            };

            await _patient.AddAsync(entity);

            return new PatientDto(
                entity.Id,
                entity.FullName,
                entity.DateOfBirth,
                entity.Email
            );
        }

        public async Task<bool> UpdateAsync(Guid id, UpdatePatientDto dto)
        {
            var patient = await _patient.GetAsync(id);
            if (patient is null) return false;

            patient.FullName = dto.FullName;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.Email = dto.Email;

            await _patient.UpdateAsync(patient);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var patient = await _patient.GetAsync(id);
            if (patient is null) return false;

            await _patient.DeleteAsync(id);
            return true;
        }
    }
}