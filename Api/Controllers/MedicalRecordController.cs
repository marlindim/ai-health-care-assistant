

using Core.Contract;
using Core.Entities;
using Infrastructure.SymptomChecker;
using Microsoft.AspNetCore.Mvc;
using static Core.Dtos.MedicalRecordSumDto;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly MedicalRecordService _service;
        private readonly IAIClient _aiClient;

        public MedicalRecordsController(MedicalRecordService service, IAIClient aiClient)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _aiClient = aiClient ?? throw new ArgumentNullException(nameof(aiClient));
        }

        /// <summary>
        /// Save a plain text medical record (will be encrypted before storing).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MedicalRecord>> SaveRecord([FromForm] string record, Guid PatientI)
        {
            if (string.IsNullOrWhiteSpace(record))
                return BadRequest("Record cannot be empty.");

            var saved = await _service.SaveRecordAsync(record, PatientI);
            return CreatedAtAction(nameof(GetRecord), new { id = saved.Id }, saved);
        }

        /// <summary>
        /// Get a decrypted medical record by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<string>> GetRecord(Guid id)
        {
            var record = await _service.GetRecordAsync(id);
            if (record == null)
                return NotFound("Record not found.");

            return Ok(record);
        }

        /// <summary>
        /// Generate and save a summary for a medical record using AI.
        /// </summary>
        [HttpPost("{id:guid}/summarize")]
        public async Task<ActionResult<MedicalRecordSummaryDto>> SummarizeRecord(Guid id)
        {
            var summarized = await _service.SummarizeAndSaveRecordAsync(id, _aiClient);
            if (summarized == null)
                return NotFound("Record not found or summarization failed.");

            var summary = await _service.GetSummaryAsync(id);
            if (string.IsNullOrWhiteSpace(summary))
                return BadRequest("Summary could not be generated.");

            return Ok(new MedicalRecordSummaryDto(summarized.Id, summary));
        }

        /// <summary>
        /// Get a decrypted summary of the medical record by ID.
        /// </summary>
        [HttpGet("{id:guid}/summary")]
        public async Task<ActionResult<MedicalRecordSummaryDto>> GetSummary(Guid id)
        {
            var summary = await _service.GetSummaryAsync(id);
            if (string.IsNullOrWhiteSpace(summary))
                return NotFound("Summary not found.");

            return Ok(new MedicalRecordSummaryDto(id, summary));
        }
    }
}


//using Core.Contract;
//using Core.Entities;
//using Infrastructure.SymptomChecker;
//using Microsoft.AspNetCore.Mvc;
//using static Core.Dtos.MedicalRecordSumDto;

//namespace Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class MedicalRecordsController : ControllerBase
//    {
//        private readonly MedicalRecordService _service;
//        private readonly IAIClient _aiClient;

//        public MedicalRecordsController(MedicalRecordService service, IAIClient aiClient)
//        {
//            _service = service;
//            _aiClient = aiClient;
//        }

//        /// <summary>
//        /// Save a plain text medical record (encrypted before saving).
//        /// </summary>
//        [HttpPost]
//        public async Task<ActionResult<MedicalRecord>> SaveRecord([FromBody] string record)
//        {
//            if (string.IsNullOrWhiteSpace(record))
//                return BadRequest("Record cannot be empty.");

//            var saved = await _service.SaveRecordAsync(record);
//            return CreatedAtAction(nameof(GetRecord), new { id = saved.Id }, saved);
//        }

//        /// <summary>
//        /// Get a decrypted medical record by ID.
//        /// </summary>
//        [HttpGet("{id:guid}")]
//        public async Task<ActionResult<string>> GetRecord(Guid id)
//        {
//            var record = await _service.GetRecordAsync(id);
//            if (record == null) return NotFound();

//            return Ok(record);
//        }

//        /// <summary>
//        /// Summarize and save an encrypted medical record summary.
//        /// </summary>
//        [HttpPost("{id:guid}/summarize")]
//        public async Task<ActionResult<MedicalRecordSummaryDto>> SummarizeRecord(Guid id)
//        {
//            var record = await _service.SummarizeAndSaveRecordAsync(id, _aiClient);
//            if (record == null) return NotFound();

//            var summary = await _service.GetSummaryAsync(id);
//            if (summary == null) return BadRequest("Summary could not be generated.");

//            return Ok(new MedicalRecordSummaryDto(record.Id, summary));
//        }

//        /// <summary>
//        /// Get a decrypted summary of the medical record.
//        /// </summary>
//        [HttpGet("{id:guid}/summary")]
//        public async Task<ActionResult<MedicalRecordSummaryDto>> GetSummary(Guid id)
//        {
//            var summary = await _service.GetSummaryAsync(id);
//            if (summary == null) return NotFound("Summary not found.");

//            return Ok(new MedicalRecordSummaryDto(id, summary));
//        }
//    }
//}
