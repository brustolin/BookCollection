using System.Text;

public static class StringSplitHelper
{
    public static string[] SplitWithEscape(this string input, char separator = ';', char escapeChar = '\\')
    {
        var parts = new List<string>();
        ReadOnlySpan<char> span = input.AsSpan();
        var sb = new StringBuilder(input.Length);

        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == escapeChar)
            {
                i++;
                if (i < span.Length)
                    sb.Append(span[i]);
            }
            else if (span[i] == separator)
            {
                parts.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(span[i]);
            }
        }

        parts.Add(sb.ToString());
        return parts.ToArray();
    }
}