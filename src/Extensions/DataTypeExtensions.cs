using System;

namespace BackupAssistant.Extensions
{
    internal static class DataTypeExtensions
    {
        internal static string ReplaceFirst(this string text, string oldValue, string newValue)
        {
            int pos = text.IndexOf(oldValue);
            return pos < 0 ? text : string.Concat(text.AsSpan(0, pos), newValue, text.AsSpan(pos + oldValue.Length));
        }
    }
}
