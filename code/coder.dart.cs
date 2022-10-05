public partial class Coder
{
    //
    // Summary:
    //   Write the dart classes from the db
    // Parameters:
    //   db:
    //     The database read
    public static void Dart(Db db)
    {

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

            File.WriteAllText($"output/models/{entity.Name.Pathize()}.dart", $@"
{string.Join("\n", eFields.Select(t => $"import './{t.Name.Pathize()}.dart';"))}

class {entity.Name.Singularize()} {{
    {string.Join("\n    ", entity.Fields.Select(t => t.ToDart()))}

    {string.Join("\n    ", childrends.Select(t => t.ToChildRelationDart()))}

    {string.Join("\n    ", fathers.Select(t => t.FatherField.ToFatherRelationDart(t.Nullable)))}

    {entity.Name.Singularize()}({{{string.Join(", ", cFields.Select(t => $"{(t.Nullable ? "" : "required ")}this.{t.Name.Normalize(true)}"))}}});
}}");
            Console.WriteLine($"Generated class {entity.Name.Singularize()}");
        }
    }
}