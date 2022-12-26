namespace PresentationApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using PresentationApi.Infrastructure.Model;
    using PresentationApi.Infrastructure.Service;

    [ApiController]
    [Route("[controller]")]
    public sealed class BankController : ControllerBase
    {
        private readonly IMessageQueueService _messageQueueService;

        public BankController(IMessageQueueService messageQueueService)
        {
            _messageQueueService = messageQueueService ?? throw new ArgumentNullException(nameof(messageQueueService));
        }

        [HttpPost(Name = "Transfer")]
        public IActionResult Transfer(TransferModel transferModel)
        {
            _messageQueueService.PublishMessage("Transfer", transferModel);

            return Ok("Transfer request successful.");
        }
    }
}