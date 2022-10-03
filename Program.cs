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

// Connecto MSSQL and fetch the structure
var db = Connector.MsSQL(config.GetConnectionString("Default"));


// Generate the DART class based on MSSQL structure
foreach (var entity in db.Tables)
{
    var childrends = entity.Fields
        .Where(t => t.ChildrendField != null)
        .Select(t => t.ChildrendField!);
    var fathers = entity.Fields
        .Where(t => t.FatherField != null)
        .Select(t => new { FatherField = t.FatherField!, t.Nullable });
    var eFields = fathers
        .Select(t => new MicroField() { Name = t.FatherField.Table.Name, Nullable = t.Nullable })
        .Union(childrends.Select(t => new MicroField() { Name = t.Table.Name, Nullable = true }));
    var cFields = entity.Fields
        .Select(t => new MicroField() { Name = t.Name, Nullable = t.Nullable })
        .Union(eFields);
    var dClass =
        $@"
{string.Join("\n", eFields.Select(t => $"import './{t.Name.Pathize(true)}.dart';"))}

class {entity.Name.Singularize()} {{
    {string.Join("\n    ", entity.Fields.Select(t => t.ToDart()))}

    {string.Join("\n    ", childrends.Select(t => t.ToChildRelationDart()))}

    {string.Join("\n    ", fathers.Select(t => t.FatherField.ToFatherRelationDart(t.Nullable)))}

    {entity.Name.Singularize()}({{{string.Join(", ", cFields.Select(t => $"{(t.Nullable ? "" : "required ")}this.{t.Name.Normalize(true)}"))}}});
}}";

    File.WriteAllText($"output/{entity.Name.Pathize(true)}.dart", dClass);
    Console.WriteLine(dClass);
}
