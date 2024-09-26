using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SKDemo.Examples._5_KernelMemory;

namespace SKDemo.API
{
    public class KernelMemoryFunction
    {
        private readonly ILogger<KernelMemoryFunction> _logger;
        private readonly KernelMem _kernelMem;

        public KernelMemoryFunction(ILogger<KernelMemoryFunction> logger)
        {
            _logger = logger;
            _kernelMem = new KernelMem();
        }

        [Function("KernelMemoryFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route ="rag")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // get the question from the request body
            string question = await new StreamReader(req.Body).ReadToEndAsync();
            
            // call KernelMem to get the answer
            var answer = await _kernelMem.Run(question);

            return new OkObjectResult(answer);
        }
    }
}
