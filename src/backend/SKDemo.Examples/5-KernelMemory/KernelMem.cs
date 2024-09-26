using Microsoft.KernelMemory;
using System.Diagnostics;
using System.Text;

namespace SKDemo.Examples._5_KernelMemory
{
    public class KernelMem
    {
        private readonly MemoryServerless memory;

        public KernelMem()
        {
            var openAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
            Debug.Assert(!string.IsNullOrEmpty(openAIKey), "OpenAIKey environment variable is not set.");

            memory = new KernelMemoryBuilder()
                .WithOpenAIDefaults(openAIKey)
                .Build<MemoryServerless>();

            // Import a file
            memory.ImportDocumentAsync("5-KernelMemory/Manta Ray corp.docx", tags: new() { { "user", "Andreas" } }).Wait();
        }

        public async ValueTask<string> Run(string question)
        {
            var answer = await memory.AskAsync(question);

            StringBuilder result = new();
            result.AppendLine($"Answer: {answer.Result}");

            //result.AppendLine("Sources:");
            //foreach (var x in answer.RelevantSources)
            //{
            //    result.AppendLine($"  - {x.SourceName}  - {x.Link} [{x.Partitions.First().LastUpdate:D}]");
            //}
            return result.ToString();
        }
    }
}
