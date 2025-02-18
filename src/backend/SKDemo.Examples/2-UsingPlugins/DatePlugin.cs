public class DatePlugin
{
    [KernelFunction]
    public string GetTodaysDate()
    {
        Console.WriteLine($"(⌐■_■) {nameof(DatePlugin)} called");

        return DateTime.Now.ToString("yyyy-MM-dd");
    }
}
