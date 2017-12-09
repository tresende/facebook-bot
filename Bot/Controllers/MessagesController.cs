using System;
using System.Threading.Tasks;
using Bot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Bot.Models;

namespace Bot.Controllers
{
    [Route("api/[controller]")]
    public class MessagesController : Controller
    {
        private readonly WebClientService webClientService;

        public MessagesController()
        {
            webClientService = new WebClientService(new Uri("https://msging.net/messages"), "d2ViaG9va3Rlc3RlMjppb2E0emhybFlZWHR0QU5uTDhMZA==");
        }

        [HttpPost]
        public async Task<OkResult> Post([FromBody]Message message)
        {
            Console.WriteLine($"Message Received: {message}");
            var numbers = message.content.Split(',');
            var num1 = int.Parse(numbers[0].Trim());
            var num2 = int.Parse(numbers[1].Trim());
            var messageContent = "\nSoma: " + (num1 + num2);
            messageContent += "\nSubtração: " + (num1 - num2);
            messageContent += "\nDivisão: " + (num1 / num2);
            messageContent += "\nMultiplicação: " + (num1 * num2);
            var replyMessage = new
            {
                id = Guid.NewGuid(),
                to = message.from,
                type = "text/plain",
                content = messageContent
            };
            await ReplyMessageAsync(replyMessage);
            return Ok();
        }

        private async Task ReplyMessageAsync(object message)
        {
            Console.WriteLine($"Message Received: {message}");
            var response = await webClientService.SendMessageAsync(message);
        }
    }
}
