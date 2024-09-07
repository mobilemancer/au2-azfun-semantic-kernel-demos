using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Diagnostics;

namespace SKDemo.Examples
{
    public class SimplePrompt
    {
        public async Task<string> Run(string draft)
        {
            var builder = Kernel.CreateBuilder();

            // Alternative using Azure OpenAI
            //builder.AddAzureOpenAIChatCompletion(
            // "gpt-4o-mini",                      // Azure OpenAI Deployment Name
            // "https://contoso.openai.azure.com/", // Azure OpenAI Endpoint
            // "Azure OpenAI Key");      // Azure OpenAI Key


            // Alternative using OpenAI
            var openAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            Debug.Assert(!string.IsNullOrEmpty(openAIKey), "OpenAIKey environment variable is not set.");
            builder.AddOpenAIChatCompletion(
                     "gpt-4o-mini",     // OpenAI Model name
                     openAIKey);        // OpenAI API Key

            var kernel = builder.Build();

            var prompt = @"{{$input}}

You are an expert job advert copy writer. 
You know everything about formatting a job advert so that people want to apply for the job. 
You make awesome job adverts from rough drafts!";

            var result = await kernel.InvokePromptAsync(prompt,
                new()
                {
                    ["input"] = draft
                });

            //var copywriterFunction = kernel.CreateFunctionFromPrompt(prompt,
            //    executionSettings: new OpenAIPromptExecutionSettings
            //    {
            //        MaxTokens = 5000,
            //        Temperature = 0.01,
            //        TopP = 0.01
            //    });
            //var result = await kernel.InvokeAsync(copywriterFunction, new()
            //{
            //    ["input"] = draft
            //});

            return result.ToString();
        }
    }
}
