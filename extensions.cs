public static class Ext
{
    //
    // Summary:
    //   Replace " " char with "_" and make the first letter as lowercase if requested
    // Parameters:
    //   value:
    //     the string to normalize
    //   lower:
    //     condition to make the first letter lowercase
    // Returns:
    //     A string normalized
    public static string Normalize(this string value, bool lower = false)
    {
        var res = value.Replace(" ", "_");
        if (!lower) return res;
        return res[0].ToString().ToLowerInvariant() + res.Substring(1);
    }

    //
    // Summary:
    //   Stub function that try to singularize a string (remove ies o s). The input will be normalized
    // Parameters:
    //   value:
    //     the string to singularize
    //   lower:
    //     condition to make the first letter lowercase
    // Returns:
    //     A string singularized
    public static string Singularize(this string value, bool lower = false)
    {
        value = value.Normalize(lower);
        if (value.EndsWith("ies")) return value.Substring(0, value.Length - 3) + "y";
        if (value.EndsWith("s")) return value.Substring(0, value.Length - 1);
        return value;
    }

    //
    // Summary:
    //   Funtion that make the string lowercase_with_underscore
    // Parameters:
    //   value:
    //     the string to PATHize
    //   lower:
    //     condition to make the first letter lowercase
    // Returns:
    //     A string PATHized
    public static string Pathize(this string value)
    {
        value = value.Singularize(false);
        var res = "";
        foreach (var ch in value)
        {
            if (System.Char.IsUpper(ch))
            {
                res += "_";
            }
            res += ch.ToString();
        }
        return res.ToLowerInvariant().Replace("__", "_");
    }
}