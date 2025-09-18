using Core.Dtos;

namespace Core.Contract
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllAsync();
        Task<PatientDto?> GetByIdAsync(Guid id);
        Task<PatientDto> CreateAsync(CreatePatientDto dto);
        Task<bool> UpdateAsync(Guid id, UpdatePatientDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
