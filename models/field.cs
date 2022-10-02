//
// Summary:
//   Superclass of minimal field data
public class MicroField
{
    public string Name { get; set; }
    public bool Nullable { get; set; }
}

//
// Summary:
//   Field of a table
public class Field : MicroField
{
    public DartType.DartFieldType Type { get; set; }

    public Field? ChildrendField { get; set; }
    public Field? FatherField { get; set; }

    public Table Table { get; set; }

    public Field(Table table, string name)
    {
        this.Table = table;
        this.Name = name;
    }

    public override string ToString()
    {
        return $"{Name} of {Table.Name}";
    }

    // Summary:
    //   Convert the field to a dart property
    // Result:
    //   A string formatted
    public string ToDart()
    {
        return $"{DartType.GetName(Type)}{(Nullable ? "?" : "")} {Name.Replace(" ", "_").ToLower()};";
    }

    // Summary:
    //   Convert the field to a dart property
    // Result:
    //   A string formatted
    public string ToChildRelationDart()
    {
        return $"List<{Table.Name.Replace(" ", "_")}>? {Table.Name.Replace(" ", "_").ToLower()};";
    }

    // Summary:
    //   Convert the field to a dart property
    // Parameters:
    //   nullable:
    //      set the property as nullable
    // Result:
    //   A string formatted
    public string ToFatherRelationDart(bool nullable)
    {
        return $"{Table.Name.Replace(" ", "_")}{(nullable ? "?" : "")} {Table.Name.Replace(" ", "_").ToLower()};";
    }
}

//
// Summary:
//   Class used to convert the enum to the keyword
public class DartType
{
    public static string INT = "int";
    public static string DOUBLE = "double";
    public static string BOOL = "bool";
    public static string STRING = "String";
    public static string DATETIME = "DateTime";

    //
    // Summary:
    //   Use reflection to convert the enum to the keyword
    // Parameters:
    //   type:
    //      the enum to convert
    // Results:
    //  The keyword of dart type

    public static string GetName(DartFieldType type)
    {
        return typeof(DartType).GetField(type.ToString()).GetValue(null).ToString();
    }

    public enum DartFieldType
    {
        INT,
        DOUBLE,
        BOOL,
        STRING,
        DATETIME
    }
}
