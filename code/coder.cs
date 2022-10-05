public partial class Coder
{
    private String _package { get; set; }
    private bool RelationNullable { get; set; }
    private Db Db { get; set; }

    public String Package => _package;

    //
    // Summary:
    //   Init the generator with Serenity parameters
    // Parameters:
    //   package:
    //     the package name of the application
    //   db:
    //     the read database
    //   allNullable:
    //     make all field nullable if true
    private Coder(string package, Db db, bool relationNullable)
    {
        this._package = package;
        this.Db = db;
        this.RelationNullable = relationNullable;
    }

    //
    // Summary:
    //   Init the Generator with Serenity parameters
    // Parameters:
    //   db:
    //     the read database
    //   allNullable:
    //     make all field nullable if true
    // Returns:
    //   the coder
    public static Coder Build(Db db, string? package = null, bool? relationNullable = null)
    {
        if (string.IsNullOrEmpty(package))
        {
            Console.WriteLine("Insert app package name [sample]");
            package = Console.ReadLine().WithDefault("sample").Trim();
        }
        if (relationNullable == null)
        {
            Console.WriteLine("Relations all nullable? [y/N]");
            relationNullable = Console.ReadLine().WithDefault("N").Trim().StartsWith("y", true, System.Globalization.CultureInfo.InvariantCulture);
        }
        return new Coder(package, db, relationNullable ?? false);
    }
}