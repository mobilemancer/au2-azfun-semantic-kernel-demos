using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Diagnostics;
using System.Text;

namespace SKDemo.Examples._4_MultiAgent
{
#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    public class MultiAgentDemo
    {
        private static OpenAIPromptExecutionSettings _openAIPromptExecutionSettings;
        private ILogger _logger;

        public MultiAgentDemo(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string?> Run(string userInput)
        {
            var chat = SetupMultiAgentChat();

            var result = new StringBuilder();
            try
            {
                // Add user input
                var _userContent = new ChatMessageContent(AuthorRole.User, userInput);
                _userContent.AuthorName = "user";
                chat.AddChatMessage(_userContent);

                await foreach (var content in chat.InvokeAsync())
                {
                    _logger.LogInformation($"{content.AuthorName ?? "Author unknown"} > {content.Content}");
                    result.AppendLine($"{content.AuthorName ?? "Author unknown"} > {content.Content}");
                    result.AppendLine();
                }
            }
            catch (Exception ex)
            {
                result.AppendLine($"An error occurred: {ex.Message}");
                _logger.LogError($"An error occurred: {ex.Message}");
            }
            return result.ToString();

        }


        public AgentGroupChat SetupMultiAgentChat()
        {
            var builder = Kernel.CreateBuilder();

            // using OpenAI
            var openAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            Debug.Assert(!string.IsNullOrEmpty(openAIKey), "OpenAIKey environment variable is not set.");

            builder.AddOpenAIChatCompletion(
                     "gpt-3.5-turbo",            // OpenAI Model name
                     //"gpt-4",
                     //"gpt-4-turbo",
                     //"gpt-4o-mini",
                     openAIKey);                 // OpenAI API Key

            var kernel = builder.Build();

            _openAIPromptExecutionSettings = new()
            {
                ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions,
                Temperature = 1.0
            };

            var moderator = Moderator(kernel);
            var lemmy = Lemmy(kernel);
            var lars = Lars(kernel);
            var kurt = Kurt(kernel);

            AgentGroupChat chat = new(lemmy, lars, kurt, moderator)
            {
                ExecutionSettings =
                new()
                {
                    // Here a TerminationStrategy subclass is used that will terminate when
                    // an assistant message contains the term "approve".
                    TerminationStrategy =
                        new ApprovalTerminationStrategy()
                        {
                            // The agent who get's to say when we are done
                            Agents = [moderator],
                            // Limit total number of turns (tweak this if the model starts producing 500's)
                            MaximumIterations = 16,
                            AutomaticReset = true
                        }
                }
            };

            return chat;
        }

        public static ChatCompletionAgent Moderator(Kernel kernel)
        {
            return new ChatCompletionAgent()
            {
                Instructions = """
You are the debate moderator.
You can decide if the chat is over or if more discussion is needed.
You should encourage the other agents to motivate their answers for at least one round.
You will decide a winner from the other agents answers when the chat is over, based on the arguments presented.
When you decide it's over, just answer "I proclaim the winner is {name of the agent} with {the agents sugestion} - the debate is now finished."
NEVER PRODUCE INVALID CONTENT!
""",
                Name = "The_Moderator",
                Kernel = kernel,
                ExecutionSettings = _openAIPromptExecutionSettings,
            };
        }

        public static ChatCompletionAgent Lemmy(Kernel kernel)
        {
            string instructions = """
You are the ghost of the late Lemmy Kilmister. 
Songwriter, singer and base player extraordinare of the band Motörhead. 
You know everything there is to know about rock'n'roll and the lifestyle!
When asked for who's the greatest band ever, you will give just one band as an answer!
NEVER PRODUCE INVALID CONTENT!
""";
            ChatCompletionAgent lemmy = new()
            {
                Instructions = instructions,
                Name = "Lemmy_Kilmister",
                Kernel = kernel
            };

            return lemmy;
        }

        public static ChatCompletionAgent Lars(Kernel kernel)
        {
            string instructions = """
You are Lars Ulrich. 
Drummer and co-founder of the band Metallica. 
You know everything there is to know about speed metall and different metall genres!
When asked for whos the greatest band ever, you will give just one band as an answer!
NEVER PRODUCE INVALID CONTENT!
""";
            ChatCompletionAgent lars = new()
            {
                Instructions = instructions,
                Name = "Lars_Ulrich",
                Kernel = kernel
            };

            return lars;
        }

        public static ChatCompletionAgent Kurt(Kernel kernel)
        {
            string instructions = """
You are the ghost of the late Kurt Cobain. 
Voice of a generation and the lead vocalist, guitarist, primary songwriter, and a founding member of the grunge band Nirvana. 
You know everything there is to know about the rock'n'roll revolution!
When asked for whos the greatest band ever, you will give just one band as an answer!
NEVER PRODUCE INVALID CONTENT!
""";

            ChatCompletionAgent kurt = new()
            {
                Instructions = instructions,
                Name = "Kurt_Cobain",
                Kernel = kernel
            };

            return kurt;
        }

        private sealed class ApprovalTerminationStrategy : TerminationStrategy
        {
            // Terminate when the final message is "finished"
            protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<ChatMessageContent> history, CancellationToken cancellationToken)
                => Task.FromResult(history[history.Count - 1].Content?.Contains("the debate is now finished", StringComparison.OrdinalIgnoreCase) ?? false);
        }
    }

#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
