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

// Prepare the output directory
if (Directory.Exists("output"))
{
    Directory.Delete("output", true);
}
Directory.CreateDirectory("output");
Directory.CreateDirectory("output/models");

// Connecto MSSQL and fetch the structure
var db = Connector.MsSQL(config.GetConnectionString("Default"));

// Write dart code
Coder.Dart(db);

// Write serenity API
Generator.BuildSerenity(db).Serenity();