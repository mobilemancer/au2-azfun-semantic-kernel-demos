using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SKDemo.Examples._4_MultiAgent;

namespace SKDemo.API
{
    public class MultiAgentChatFunction
    {
        private readonly ILogger<MultiAgentChatFunction> _logger;

        public MultiAgentChatFunction(ILogger<MultiAgentChatFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(MultiAgentChatFunction))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "agentchat")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // get the input from the request body
            string input = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation(input);

            // call AgentsService to make a revised draft
            var agentChat = await new MultiAgentDemo(_logger).Run(input);

            return new OkObjectResult(agentChat);
        }
    }
}
