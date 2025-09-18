using Core.Contract;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientsController(IPatientService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PatientDto>> GetPatient(Guid id)
        {
            var patient = await _service.GetByIdAsync(id);
            if (patient is null) return NotFound();

            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<PatientDto>> CreatePatient([FromForm] CreatePatientDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPatient), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdatePatient(Guid id, UpdatePatientDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeletePatient(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
