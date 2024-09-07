using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SKDemo.Examples._2_RefineDraft;
using SKDemo.Examples._2_UsingPlugins;
using System.Diagnostics;

namespace SKDemo.Examples
{
    public class RefineDraft
    {
        private Kernel _kernel;
        public RefineDraft()
        {
            var builder = Kernel.CreateBuilder();

            // using OpenAI
            var openAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            Debug.Assert(!string.IsNullOrEmpty(openAIKey), "OpenAIKey environment variable is not set.");

            builder.AddOpenAIChatCompletion(
                     "gpt-4o-mini",             // OpenAI Model name
                     openAIKey);                // OpenAI API Key

            builder.Plugins.AddFromType<JobAddCopywriterPlugin>();
            builder.Plugins.AddFromType<MarkdownToHTMLPlugin>();
            builder.Plugins.AddFromType<DatePlugin>();

            _kernel = builder.Build();
        }

        public async Task<string> Run(string draft)
        {
            var prompt = @"{{$input}}

You are responsible to turning drafts of job descriptions into a perfect job advert formated in HTML.
Make sure to add todays date in the top right corner of the page.
You will only respond with the finished HTML, no other comments and no ´´´html tags.";

            var jobPageCreator = _kernel.CreateFunctionFromPrompt(prompt,
                executionSettings: new OpenAIPromptExecutionSettings
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                    MaxTokens = 5000
                });

            var result = await _kernel.InvokeAsync(jobPageCreator, new()
            {
                ["input"] = draft
            });

            Console.WriteLine(result);

            return result.ToString();
        }
    }



}
