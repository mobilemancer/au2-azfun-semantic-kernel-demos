using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;

namespace SKDemo.Examples._2_RefineDraft
{
    public class JobAddCopywriterPlugin
    {
        [KernelFunction, Description("Convert a draft to a great job add")]
        public async Task<string> ConvertAsync(Kernel kernel, [Description("Draft of a job description to turn into an add")] string draft)
        {
            Console.WriteLine($"(⌐■_■) {nameof(JobAddCopywriterPlugin)} called");

            var prompt = @"{{$input}}

You are an expert job advert copy writer. 
You know everything about formatting a job advert so that people want to apply for the job. 
You make awesome job adverts from rough drafts!
You return the result in semantically correct markdown format, that is optimized for perfect SEO ranking";

            var summarize = kernel.CreateFunctionFromPrompt(prompt, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 1000 });

            var result = await kernel.InvokeAsync(summarize, new()
            {
                ["input"] = draft
            });

            Console.WriteLine(result);

            return result.ToString();
        }
    }
}
