using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressService.Models.Dto;
using ProgressService.Services;
using ProgressService.Services.Interfaces;
using ProgressService.Services.Templates;

namespace ProgressService.Controllers
{
    [ApiController]
    [Route("api/sms")]
    [Authorize(Policy = "AdminOnly")]
    public class SmsController : ControllerBase
    {
        private readonly ISmsService _smsService;
        public SmsController(ISmsService smsService) => _smsService = smsService;

        [HttpPost("send")]
        public async Task<IActionResult> SendCustomSms(
            [FromBody] SendSmsRequest dto)
        {
            //for now send to this number, in the future to any number 
            await _smsService.SendAsync(
                    toE164: "+16479151261",
                    message: dto.Message,
                    correlationId: Guid.NewGuid().ToString("N"),
                    idempotencyKey: Guid.NewGuid().ToString("N")
                );

            return Ok();
        }


    }
}
