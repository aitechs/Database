public static class Extension
{
    public static bool IsNumeric(this string text)
    {
        if (string.IsNullOrEmpty(text)) return false;

        double number;
        return double.TryParse(text, out number);
    }
}
