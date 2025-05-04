namespace MiniTycoonPiekarnia.Models;

public static class FileNameHelper
{
    public static string Normalize(string input) =>
        input.ToLower()
             .Replace("ą", "a").Replace("ć", "c").Replace("ę", "e")
             .Replace("ł", "l").Replace("ń", "n").Replace("ó", "o")
             .Replace("ś", "s").Replace("ź", "z").Replace("ż", "z")
             .Replace(" ", "");
}
