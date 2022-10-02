/*
1. Connect to database
2. Read all tables
3. For each table read all fields
4. Understand connections
5. Write interface
*/

// Prepare the output directory
if (Directory.Exists("output"))
{
    Directory.Delete("output", true);
}
Directory.CreateDirectory("output");

// Connecto MSSQL and fetch the structure
var db = Connector.MsSQL(
    "SQL6020.site4now.net",
    "db_a26160_northwind_admin",
    "northwind1",
    "db_a26160_northwind"
);
Console.Clear();

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
{string.Join("\n", eFields.Select(t => $"import './{t.Name.Replace(" ", "_").ToLower()}.dart';"))}

class {entity.Name.Replace(" ", "_")} {{
    {string.Join("\n    ", entity.Fields.Select(t => t.ToDart()))}

    {string.Join("\n    ", childrends.Select(t => t.ToChildRelationDart()))}

    {string.Join("\n    ", fathers.Select(t => t.FatherField.ToFatherRelationDart(t.Nullable)))}

    {entity.Name.Replace(" ", "_")}({{{string.Join(", ", cFields.Select(t => $"{(t.Nullable ? "" : "required ")}this.{t.Name.Replace(" ", "_").ToLower()}"))}}}){{}}
}}";

    File.WriteAllText($"output/{entity.Name.Replace(" ", "_").ToLower()}.dart", dClass);
    Console.WriteLine(dClass);
}
