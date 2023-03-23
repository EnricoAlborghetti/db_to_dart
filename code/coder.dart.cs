public partial class Coder
{
    //
    // Summary:
    //   Write the dart classes from the db
    // Parameters:
    //   db:
    //     The read database
    public void Dart()
    {
        Directory.CreateDirectory("output/models");
        Directory.CreateDirectory("output/models/filters");
        if (Api)
        {
            Directory.CreateDirectory("output/models/api");
            System.IO.File.WriteAllText("output/models/api/json_factory.dart", $@"
abstract class JsonFactory<T> {{
  Map<String, dynamic> toJson();
}}");
            System.IO.File.WriteAllText("output/models/api/json_serializer.dart", $@"
import 'package:{Package}/models/api/json_factory.dart';

abstract class JsonSerializer<T extends JsonFactory> {{
  T fromJson(Map<String, dynamic> json);
}}");
        }

        // Generate the DART class based on MSSQL structure
        foreach (var entity in Db.Tables)
        {
            var childrends = entity.Fields
                .Where(t => t.ChildrendField != null)
                .Select(t => t.ChildrendField!);
            var fathers = entity.Fields
                .Where(t => t.FatherField != null)
                .Select(t => new { FatherField = t.FatherField!, Nullable = t.Nullable || RelationNullable });
            var fFields = fathers
                .Select(t => new MicroField() { Name = t.FatherField.Table.Name.Singularize(), Nullable = t.Nullable });
            var ccFields = childrends.Select(t => new MicroField() { Name = t.Table.Name, Nullable = true });
            var eFields = fFields.Union(ccFields);
            var cFields = entity.Fields
                .Select(t => new MicroField() { Name = t.Name, Nullable = t.Nullable })
                .Union(eFields);

            var filterFields = entity.Fields.Where(t => !t.PrimaryKey);

            var apiPart = "";
            var apiPart2 = "";
            if (Api)
            {
                apiPart2 = @$"class {entity.Name.Singularize()} extends {entity.Name.Singularize()}Base {{

    {string.Join("\n    ", entity.Fields.Where(x => !x.Nullable).Select(t => t.ToGetProperty()))}


    {entity.Name.Singularize()}({{{string.Join(", ", cFields.Select(t => $"{(t.Nullable ? "" : "required ")}super.{t.Name.Normalize(true)}"))}}});

    
    {entity.Name.Singularize()}.fromJson(Map<String, dynamic> json){{
        {string.Join("\n        ", entity.Fields.Select(t => t.ParseType()))}
        {string.Join("\n        ", ccFields.Select(t => $"if (json['{t.Name}'] != null) {{ {t.Name.Normalize(true)} = []; json[\"{t.Name}\"].forEach((v) => {t.Name.Normalize(true)}!.add({t.Name.Singularize()}.fromJson(v))); }}"))}
        {string.Join("\n        ", fFields.Select(t => $"if (json['{t.Name}'] != null) {{ {t.Name.Singularize(true)} = {t.Name.Singularize()}.fromJson(json['{t.Name}']); }}"))}
    }}

    @override
  Map<String, dynamic> toJson() {{
    final Map<String, dynamic> jData = <String, dynamic>{{}};
    {string.Join("\n    ", entity.Fields.Select(t => $"jData['{t.Name}'] = {t.Name.Normalize(true) + (t.Type == DartType.DartFieldType.DATETIME ? (t.Nullable  ? "?" : "") + ".toIso8601String()" : "")};"))}
    //{string.Join("\n    ", childrends.Select(t => $"if ({t.Table.Name.Normalize(true)} != null) {{ jData['{t.Table.Name.Normalize(true)}'] = {t.Table.Name.Normalize(true)}!.map((v) => v.toJson()).toList(); }}"))}
    //{string.Join("\n    ", fathers.Select(t => $"if ({t.FatherField.Table.Name.Singularize(true)} != null) {{ jData['{t.FatherField.Table.Name.Singularize(true)}'] = {t.FatherField.Table.Name.Singularize(true)}!.toJson(); }}"))}
    return jData;
  }}

}}";

                apiPart = @$"
    {entity.Name.Singularize()}Base.fromJson(Map<String, dynamic> json){{
        {string.Join("\n        ", entity.Fields.Select(t => t.ParseType()))}
        {string.Join("\n        ", ccFields.Select(t => $"if (json['{t.Name}'] != null) {{ {t.Name.Normalize(true)} = []; json[\"{t.Name}\"].forEach((v) => {t.Name.Normalize(true)}!.add({t.Name.Singularize()}.fromJson(v))); }}"))}
        {string.Join("\n        ", fFields.Select(t => $"if (json['{t.Name}'] != null) {{ {t.Name.Singularize(true)} = {t.Name.Singularize()}.fromJson(json['{t.Name}']); }}"))}
    }}

    @override
  Map<String, dynamic> toJson() {{
    final Map<String, dynamic> jData = <String, dynamic>{{}};
    {string.Join("\n    ", entity.Fields.Select(t => $"if ({t.Name.Normalize(true)} != null) {{ jData['{t.Name}'] = {t.Name.Normalize(true) + (t.Type == DartType.DartFieldType.DATETIME ? (true ? "?" : "") + ".toIso8601String()" : "")}; }}"))}
    //{string.Join("\n    ", childrends.Select(t => $"if ({t.Table.Name.Normalize(true)} != null) {{ jData['{t.Table.Name.Normalize(true)}'] = {t.Table.Name.Normalize(true)}!.map((v) => v.toJson()).toList(); }}"))}
    //{string.Join("\n    ", fathers.Select(t => $"if ({t.FatherField.Table.Name.Singularize(true)} != null) {{ jData['{t.FatherField.Table.Name.Singularize(true)}'] = {t.FatherField.Table.Name.Singularize(true)}!.toJson(); }}"))}
    return jData;
  }}";


                File.WriteAllText($"output/models/filters/{entity.Name.Pathize()}_filter.dart", $@"
import 'package:{Package}/models/serenity/filter.dart';
import 'package:{Package}/models/api/json_factory.dart';

class {entity.Name.Singularize()}Comparer extends JsonFactory {{
    {string.Join("\n    ", filterFields.Select(t => t.ToDart(false, true)))}

    {entity.Name.Singularize()}Comparer({{{string.Join(", ", filterFields.Select(t => $"this.{t.Name.Normalize(true)}"))}}});

    @override
    Map<String, dynamic> toJson() {{
        final Map<String, dynamic> jData = <String, dynamic>{{}};
        {string.Join("\n    ", filterFields.Select(t => $"if ({t.Name.Normalize(true)} != null) {{ jData['{t.Name.Normalize(true)}'] = {t.Name.Normalize(true) + (t.Type == DartType.DartFieldType.DATETIME ? "!.toIso8601String()" : "")};}}"))}
        return jData;
    }}

}}

class {entity.Name.Singularize()}Filter extends FilterT<{entity.Name.Singularize()}Comparer> {{

    {entity.Name.Singularize()}Filter({{super.take, super.skip, super.customData, super.includeColumns, super.equalityFilter}});
}}");
            }


            File.WriteAllText($"output/models/{entity.Name.Pathize()}.dart", $@"
import 'package:{Package}/models/api/json_factory.dart';
{string.Join("\n", eFields.Where(t => t.Name != entity.Name).Select(t => $"import './{t.Name.Pathize()}.dart';"))}

class {entity.Name.Singularize()}Base {(Api ? $"extends JsonFactory" : "")} {{
    {string.Join("\n    ", entity.Fields.Select(t => t.ToDart(Api, true)))}

    {string.Join("\n    ", childrends.Select(t => t.ToChildRelationDart()))}

    {string.Join("\n    ", fathers.Select(t => t.FatherField.ToFatherRelationDart(t.Nullable)))}

    {entity.Name.Singularize()}Base({{{string.Join(", ", cFields.Select(t => $"this.{t.Name.Normalize(true)}"))}}});

    {apiPart}

    List<String> getPrimaryKeys() => ['{string.Join("','", entity.Fields.Where(t => t.PrimaryKey).Select(t => t.Name))}'];
}}

{apiPart2}
");
            Console.WriteLine($"Generated class {entity.Name.Singularize()}");
        }
    }
}