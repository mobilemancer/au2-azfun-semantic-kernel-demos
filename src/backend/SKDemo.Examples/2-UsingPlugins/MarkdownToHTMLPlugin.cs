public class MarkdownToHTMLPlugin
{
    [KernelFunction, Description("Convert markdown to HTML")]
    public static string Convert([Description("Markdown text to convert")] string markdown)
    {
        Console.WriteLine($"(⌐■_■) {nameof(MarkdownToHTMLPlugin)} called");

        var result = new Markdown().Transform(markdown);

        Console.WriteLine(result);
        return result;
    }
}
