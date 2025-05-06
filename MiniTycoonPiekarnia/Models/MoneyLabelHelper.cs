namespace MiniTycoonPiekarnia.Models;

public static class MoneyLabelHelper
{
    public static string GetLabel(long amount)
    {
        if (amount == 1)
            return "peks";

        int lastTwoDigits = (int)(amount % 100);
        int lastDigit = (int)(amount % 10);

        if (lastTwoDigits >= 12 && lastTwoDigits <= 14)
            return "peksów";

        return lastDigit switch
        {
            2 or 3 or 4 => "peksy",
            _ => "peksów"
        };
    }

    public static string Format(long amount)
    {
        return $"{amount} {GetLabel(amount)}";
    }

    public static string Format(decimal amount)
    {
        bool isWhole = amount == Math.Floor(amount);

        if (isWhole)
        {
            return $"{amount:0.##} {GetLabel((long)amount)}";
        }
        else
        {
            return $"{amount:0.##} peksów";
        }
    }

    public static string GetLabel(decimal amount)
    {
        return amount == Math.Floor(amount)
            ? GetLabel((long)amount)
            : "peksów";
    }

    public static string Format(int amount)
    {
        return Format((long)amount);
    }

    public static string GetLabel(int amount)
    {
        return GetLabel((long)amount);
    }
}
