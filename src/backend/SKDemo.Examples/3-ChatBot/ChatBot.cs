using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SKDemo.Examples._2_RefineDraft;
using System.Diagnostics;

namespace SKDemo.Examples._3_ChatBot
{
    public class ChatBot
    {
        private static Kernel _kernel;
        private static ChatHistory _history;
        private static IChatCompletionService _chatCompletionService;
        private static OpenAIPromptExecutionSettings _openAIPromptExecutionSettings;

        public ChatBot()
        {
            if (_kernel == null)
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

                _kernel = builder.Build();

                // Create chat history
                _history = new ChatHistory();

                // Get chat completion service
                _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

                // Enable auto function calling
                _openAIPromptExecutionSettings = new()
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                };

            }
        }

        public async Task<string?> Run(string userInput)
        {
            // Add user input
            _history.AddUserMessage(userInput);

            // Get the response from the AI
            var result = await _chatCompletionService.GetChatMessageContentAsync(
                _history,
                executionSettings: _openAIPromptExecutionSettings,
                kernel: _kernel);

            // Add the message from the agent to the chat history
            _history.AddMessage(result.Role, result.Content ?? string.Empty);

            return result.Content;
        }
    }
}
