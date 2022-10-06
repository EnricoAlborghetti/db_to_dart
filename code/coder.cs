public partial class Coder
{
    private String _package { get; set; }
    private bool RelationNullable { get; set; }
    private bool Api { get; set; }
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
    //   api:
    //     create the API structure if true
    private Coder(string package, Db db, bool relationNullable, bool api)
    {
        this._package = package;
        this.Db = db;
        this.RelationNullable = relationNullable;
        this.Api = api;
    }

    //
    // Summary:
    //   Init the Generator with Serenity parameters
    // Parameters:
    //   db:
    //     the read database
    //   allNullable:
    //     make all field nullable if true
    //   api:
    //     create the api structure if true
    // Returns:
    //   the coder
    public static Coder Build(Db db, string? package = null, bool? relationNullable = null, bool? api = null)
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
        if (api == null)
        {
            Console.WriteLine("Create API structure? [y/N]");
            api = Console.ReadLine().WithDefault("N").Trim().StartsWith("y", true, System.Globalization.CultureInfo.InvariantCulture);
        }
        return new Coder(package, db, relationNullable ?? false, api ?? false);
    }
}