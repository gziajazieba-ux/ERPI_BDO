using System.Text;

public static class DebugLogger
{
    private static readonly StringBuilder _buffer = new();

    public static void Add(string text)
    {
        _buffer.AppendLine($"[{DateTime.Now:HH:mm:ss}] {text}");
    }

    public static string GetText()
    {
        return _buffer.ToString();
    }

    public static void Clear()
    {
        _buffer.Clear();
    }
}
