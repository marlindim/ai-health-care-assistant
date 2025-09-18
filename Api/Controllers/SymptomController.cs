using Infrastructure.SymptomChecker;
using Microsoft.AspNetCore.Mvc;
using static Infrastructure.AIqueryResponse.CheckResponse;

namespace Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class SymptomController : ControllerBase
    {
        private readonly SymptomCheckerService _service;
        public SymptomController(SymptomCheckerService service) => _service = service;

        [HttpPost("check")]
        public async Task<IActionResult> Check([FromBody] SymptomCheckRequest req)
        {
            var res = await _service.CheckAsync(req);
            return Ok(res);
        }
    }
}
