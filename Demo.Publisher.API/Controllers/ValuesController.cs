namespace Demo.Publisher.API.Controllers
{
    using Demo.Publisher.API.Services;
    using Microsoft.AspNetCore.Mvc;
    using System;

    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public ValuesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        [Consumes("text/plain")]
        public IActionResult Post([FromBody] string payload)
        {
            Console.WriteLine($"Received a Post: {payload}");
            _messageService.Enqueue(payload);

            return Ok();
        }

        [HttpGet]
        public IActionResult Get() 
        {
            return Ok();
        }
    }
}
