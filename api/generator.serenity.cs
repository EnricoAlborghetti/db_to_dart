public partial class Generator
{
    private String Package { get; set; }
    private String Module { get; set; }
    private Db Db { get; set; }

    //
    // Summary:
    //   Init a Generator with Serenity parameters
    // Parameters:
    //   db:
    //     the read database
    // Returns:
    //   the generator, can only use .Serenity()
    public static Generator BuildSerenity(Db db, string? package = null, string? module = null)
    {
        if (string.IsNullOrEmpty(package))
        {
            Console.WriteLine($"Insert app package name [sample]");
            package = Console.ReadLine().WithDefault("sample").Trim();
        }
        if (string.IsNullOrEmpty(module))
        {
            Console.WriteLine($"Insert Serenity Module Name [{package}Db]");
            module = Console.ReadLine().WithDefault(package + "Db").Trim();
        }
        return new Generator(package, module, db);
    }

    //
    // Summary:
    //   Init the generator with Serenity parameters
    // Parameters:
    //   package:
    //     the package name of the application
    //   module:
    //     the Serenity module name
    //   db:
    //     the read database
    private Generator(string package, string module, Db db)
    {
        this.Package = package;
        this.Module = module;
        this.Db = db;
    }

    //
    // Summary:
    //   Write the serenity models and services
    public void Serenity()
    {
        if (string.IsNullOrWhiteSpace(Package)) throw new Exception("Package missing, first call BuildSerenity");
        if (string.IsNullOrWhiteSpace(Module)) throw new Exception("Module missing, first call BuildSerenity");
        if (Db == null) throw new Exception("Db missing, first call BuildSerenity");

        #region static models

        Directory.CreateDirectory("output/models/serenity");
        File.WriteAllText("output/models/serenity/error.dart", @$"
import 'package:{Package}/models/api/json_factory.dart';

class Error implements JsonFactory {{
  String? code;
  String? message;

  Error({{this.code, this.message}});

  Error.fromJson(Map<String, dynamic> json) {{
    code = json['Code'];
    message = json['Message'];
  }}

  @override
  Map<String, dynamic> toJson() {{
    final Map<String, dynamic> data = <String, dynamic>{{}};
    data['Code'] = code;
    data['Message'] = message;
    return data;
  }}
}}
");
        File.WriteAllText("output/models/serenity/filter.dart", @"
class Filter {
  int take;
  List<String> includeColumns;

  Filter({required this.take, required this.includeColumns});

  Map<String, dynamic> toJson() {
    return {
      'take': take,
      'includeColumns': includeColumns,
    };
  }
}

class FilterT<T> extends Filter {
  T equalityFilter;

  FilterT(
      {required super.take,
      required super.includeColumns,
      required this.equalityFilter});
}");
File.WriteAllText("output/models/serenity/web_file_response.dart", @$"
import 'package:{Package}/models/api/json_factory.dart';

class WebFileResponse implements JsonFactory {{
  String? temporaryFile;
  int? size;
  bool? isImage;
  int? width;
  int? height;

  WebFileResponse(
      {{this.temporaryFile, this.size, this.isImage, this.width, this.height}});

  WebFileResponse.fromJson(Map<String, dynamic> json) {{
    temporaryFile = json['temporaryFile'];
    size = json['size'];
    isImage = json['isImage'];
    width = json['width'];
    height = json['height'];
  }}

  @override
  Map<String, dynamic> toJson() {{
    final Map<String, dynamic> data = <String, dynamic>{{}};
    data['temporaryFile'] = temporaryFile;
    data['size'] = size;
    data['isImage'] = isImage;
    data['width'] = width;
    data['height'] = height;
    return data;
  }}
}}");
        File.WriteAllText("output/models/serenity/web_response.dart", @$"
import 'package:{Package}/models/api/json_factory.dart';
import 'package:{Package}/models/api/json_serializer.dart';
import 'package:{Package}/models/serenity/error.dart';

class WebResponse<T extends JsonFactory> implements JsonFactory {{
  List<T>? entities;
  T? entity;
  dynamic values;
  dynamic localizations;
  int? totalCount;
  int? skip;
  int? take;
  Error? error;

  WebResponse(
      {{this.entities,
      this.entity,
      this.values,
      this.localizations,
      this.totalCount,
      this.skip,
      this.take,
      this.error}});

  WebResponse.fromJson(
      Map<String, dynamic> json, JsonSerializer<T> serializer) {{
    if (json['Entities'] != null) {{
      entities = [];
      json['Entities'].forEach((v) => entities!.add(serializer.fromJson(v)));
    }}
    if (json['Entity'] != null) {{
      entity = serializer.fromJson(json['Entity']);
    }}
    values = json['Values'];
    totalCount = json['TotalCount'];
    skip = json['Skip'];
    take = json['Take'];
    if (json['Error'] != null) {{
      error = Error.fromJson(json['Error']);
    }}
  }}

  @override
  Map<String, dynamic> toJson() {{
    final Map<String, dynamic> data = <String, dynamic>{{}};
    if (entities != null) {{
      data['Entities'] = entities!.map((v) => v.toJson()).toList();
    }}
    data['Values'] = values;
    data['TotalCount'] = totalCount;
    data['Skip'] = skip;
    data['Take'] = take;
    if (error != null) {{
      data['Error'] = error!.toJson();
    }}
    return data;
  }}
}}");
        #endregion


        #region services

        Directory.CreateDirectory("output/services");
        Directory.CreateDirectory("output/services/serenity");

        File.WriteAllText("output/services/serenity/serenity_service_factory.dart", $@"
import 'package:{this.Package}/models/serenity/filter.dart';
import 'package:{this.Package}/models/serenity/web_response.dart';
import 'package:{this.Package}/models/api/json_factory.dart';

abstract class SerenityServiceFactory<T extends JsonFactory> {{
  Future<WebResponse<T>> list({{Filter? filter}});
  Future<WebResponse<T>> delete(int entityId);
  Future<WebResponse<T>> retrieve(int entityId);
  Future<WebResponse<T>> create(T entity);

  Filter getDefaultFilter();
}}");
        File.WriteAllText("output/services/serenity/serenity_service.dart", $@"
import 'package:dio/dio.dart';
import 'package:{this.Package}/models/serenity/filter.dart';
import 'package:{this.Package}/models/serenity/web_response.dart';
import 'package:{this.Package}/models/serenity/web_file_response.dart';
import 'package:{this.Package}/models/api/json_serializer.dart';
import 'package:{this.Package}/models/api/json_factory.dart';
import 'package:{this.Package}/services/serenity/serenity_service_factory.dart';

abstract class SerenityService<T extends JsonFactory> implements SerenityServiceFactory<T> {{
  final Dio _dio = Dio(BaseOptions(
    baseUrl: 'SET_BASE_URL',
    headers: {{
      'Authorization': 'SET_AUTH',
      'Content-Type': 'application/json'
    }},
  ));

  late String apiName;
  late Filter defaultFilter;
  late JsonSerializer<T> jsonSerializer;

  SerenityService() {{
    _dio.interceptors.add(LogInterceptor(responseBody: true));
  }}

  Future<WebResponse<T>> _makeCall(String url, dynamic data) async {{
    final result = await _dio.post(url, data: data);
    return WebResponse.fromJson(result.data, jsonSerializer);
  }}

  Future<WebFileResponse> upload(File file) async {{
    final result = await _dio.post('/File/TemporaryUpload', data: file);
    return WebFileResponse.fromJson(result.data);
  }}

  @override
  Future<WebResponse<T>> list({{Filter? filter}}) async {{
    filter ??= defaultFilter;
    return _makeCall('Services/{this.Module}/$apiName/List', filter);
  }}

  @override
  Future<WebResponse<T>> delete(int entityId) async {{
    return _makeCall(
        'Services/{this.Module}/$apiName/Delete', {{'EntityId': entityId}});
  }}

  @override
  Future<WebResponse<T>> retrieve(int entityId) async {{
    return _makeCall(
        'Services/{this.Module}/$apiName/Retrieve', {{'EntityId': entityId}});
  }}

  @override
  Future<WebResponse<T>> create(T entity) async {{
    throw Exception('This should be overrided to call innerCreate');
  }}

  Future<WebResponse<T>> innerCreate(Map<String, dynamic> entity) async {{
    return _makeCall('Services/MilowDb/$apiName/Create', {{'Entity': entity}});
  }}

  @override
  Filter getDefaultFilter() {{
    return defaultFilter;
  }}
}}");

        foreach (var entity in Db.Tables)
        {
            File.WriteAllText($"output/services/{entity.Name.Pathize()}_service.dart", $@"
import 'package:{this.Package}/models/{entity.Name.Pathize()}.dart';
import 'package:{this.Package}/models/api/json_serializer.dart';
import 'package:{this.Package}/models/serenity/filter.dart';
import 'package:{this.Package}/services/serenity/serenity_service.dart';
import 'package:{this.Package}/services/serenity/serenity_service_factory.dart';
import 'package:{this.Package}/models/serenity/web_response.dart';

class {entity.Name.Singularize()}Service extends SerenityService<{entity.Name.Singularize()}> implements {entity.Name.Singularize()}ServiceFactory {{
  {entity.Name.Singularize()}Service() {{
    apiName = '{entity.Name}';
    defaultFilter = Filter(take: 100, includeColumns: ['{string.Join("','", entity.Fields.Select(t => t.Name))}']);
    jsonSerializer = {entity.Name.Singularize()}JsonSerializer();
  }}

  @override
  Future<WebResponse<{entity.Name.Singularize()}>> create({entity.Name.Singularize()} entity) async {{
    final mapping = entity.toJson();
    for (var key in entity.getPrimaryKeys()) {{
      mapping.remove(key);      
    }}
    return super.innerCreate(mapping);
  }}
}}

abstract class {entity.Name.Singularize()}ServiceFactory extends SerenityServiceFactory<{entity.Name.Singularize()}> {{}}

class {entity.Name.Singularize()}JsonSerializer extends JsonSerializer<{entity.Name.Singularize()}> {{
  @override
  {entity.Name.Singularize()} fromJson(Map<String, dynamic> json){{
    return {entity.Name.Singularize()}.fromJson(json);
  }}  
}}");
            Console.WriteLine($"Generated service for {entity.Name.Singularize()}");
        }

        #endregion
    }
}