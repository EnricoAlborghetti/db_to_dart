using Microsoft.Extensions.Configuration;

/*
1. Connect to database
2. Read all tables
3. For each table read all fields
4. Understand connections
5. Write classes
*/

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var config = builder.Build();
var GetConnectionString = config.GetConnectionString("Default");
var package = config.GetSection("Dart")["Package"] ?? null;
var relationNullable = bool.Parse(config.GetSection("Dart")["RelationNullable"] ?? "false");

// Prepare the output directory
if (Directory.Exists("output"))
{
    Directory.Delete("output", true);
}
Directory.CreateDirectory("output");

// Connecto MSSQL and fetch the structure
var db = Connector.MsSQL(config.GetConnectionString("Default"));

// Write dart code
var coder = Coder.Build(db, package, relationNullable);

coder.Dart();


var serenityModule = config.GetSection("Serenity")["Module"] ?? "";
// Write serenity API
Generator.BuildSerenity(db, coder.Package, serenityModule).Serenity();

// Write the DI file
coder.DI();