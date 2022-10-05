public partial class Coder
{
    private String _package { get; set; }
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
    private Coder(string package, Db db)
    {
        this._package = package;
        this.Db = db;
    }

    //
    // Summary:
    //   Init the Generator with Serenity parameters
    // Parameters:
    //   db:
    //     the read database
    // Returns:
    //   the coder
    public static Coder Build(Db db)
    {
        Console.WriteLine("Insert app package name [sample]");
        var package = Console.ReadLine().WithDefault("sample").Trim();

        return new Coder(package, db);
    }
}