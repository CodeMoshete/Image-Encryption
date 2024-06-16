using System;

public static class StringUtils
{
    public static string[] StringSplit(this string original, string splitTerm)
    {
        string[] array = original.Split(new string[] { splitTerm }, StringSplitOptions.None);
        return array;
    }
}
