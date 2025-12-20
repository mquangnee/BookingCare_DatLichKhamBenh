using BookingCare.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Controllers.Api
{
    [Route("api/chat")]
    [ApiController]
    public class ChatApiController : ControllerBase
    {
        private readonly IChatbot _chatService;

        public ChatApiController(IChatbot chatService)
        {
            _chatService = chatService;
        }

        public class ChatRequest
        {
            public string Message { get; set; } = "";
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Message))
                return BadRequest(new { error = "Tin nhắn rỗng." });

            var userId = HttpContext.Session.Id; // mỗi user 1 session lịch sử chat
            var reply = await _chatService.AskAsync(userId, req.Message);

            return Ok(new { reply });
        }
    }
}
