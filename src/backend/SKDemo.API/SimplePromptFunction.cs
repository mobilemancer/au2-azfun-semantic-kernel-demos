using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SKDemo.API
{
    public class SimplePromptFunction
    {
        private readonly ILogger<SimplePromptFunction> _logger;

        public SimplePromptFunction(ILogger<SimplePromptFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(SimplePromptFunction))]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "simpleprompt")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // get the draft from the request body
            string draft = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation(draft);

            // call AgentsService to make a revised draft
            var refinedDraft = await new Examples.SimplePrompt().Run(draft);

            return new OkObjectResult(refinedDraft);
        }
    }
}
