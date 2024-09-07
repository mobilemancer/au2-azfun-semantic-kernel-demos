using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SKDemo.Examples._3_ChatBot;

namespace SKDemo.API
{
    public class ChatBotFunction
    {
        private readonly ILogger<ChatBot> _logger;
        private static ChatBot _chatBot;

        public ChatBotFunction(ILogger<ChatBot> logger)
        {
            _logger = logger;
            if (_chatBot == null) _chatBot = new ChatBot();
        }

        [Function(nameof(ChatBotFunction))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chatbot")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // get the draft from the request body
            string line = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation(line);

            // call AgentsService to make a revised draft
            var chatResponse = await _chatBot.Run(line);

            return new OkObjectResult(chatResponse);
        }
    }
}
