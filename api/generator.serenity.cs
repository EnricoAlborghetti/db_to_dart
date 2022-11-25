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
  late String code;
  late String message;

  Error({{required this.code, required this.message}});

  Error.fromJson(Map<String, dynamic> json) {{
    code = json['Code'];
    message = json['Message'];
  }}

  @override
  Map<String, dynamic> toJson() {{
    final Map<String, dynamic> jData = <String, dynamic>{{}};
    jData['Code'] = code;
    jData['Message'] = message;
    return jData;
  }}
}}
");
        File.WriteAllText("output/models/serenity/filter.dart", @$"
import 'package:{Package}/models/api/json_factory.dart';

class Filter extends JsonFactory {{
  int take;
  List<String>? includeColumns;

  Filter({{this.take = 0, this.includeColumns}});

  @override
  Map<String, dynamic> toJson() {{
    final Map<String, dynamic> jData = <String, dynamic>{{}};
    jData['take'] = take;
    if (includeColumns != null) {{
      jData['includeColumns'] = includeColumns;
    }}
    return jData;
  }}
}}

class FilterT<T extends JsonFactory> extends Filter {{
  T? equalityFilter;

  FilterT(
      {{super.take,
      super.includeColumns,
      this.equalityFilter}});

  FilterT.fromFilter(Filter filter, T entity)
      : super(includeColumns: filter.includeColumns, take: filter.take);

  @override
  Map<String, dynamic> toJson() {{
    final Map<String, dynamic> jData = super.toJson();
    if (equalityFilter != null) {{
      jData['equalityFilter'] = equalityFilter!.toJson();
    }}
    return jData;
  }}
}}");
File.WriteAllText("output/models/serenity/web_file_response.dart", @$"
import 'package:{Package}/models/api/json_factory.dart';
import 'package:{Package}/models/serenity/error.dart';

class WebFileResponse implements JsonFactory {{
  String? temporaryFile;
  int? size;
  bool? isImage;
  int? width;
  int? height;
  Error? error;

  WebFileResponse(
      {{this.temporaryFile, this.size, this.isImage, this.width, this.height}});

  WebFileResponse.fromJson(Map<String, dynamic> json) {{
    temporaryFile = json['TemporaryFile'];
    size = json['Size'];
    isImage = json['IsImage'];
    width = json['Width'];
    height = json['Height'];
    if (json['Error'] != null) {{
      error = Error.fromJson(json['Error']);
    }}
  }}

  @override
  Map<String, dynamic> toJson() {{
    final Map<String, dynamic> jData = <String, dynamic>{{}};
    jData['TemporaryFile'] = temporaryFile;
    jData['Size'] = size;
    jData['IsImage'] = isImage;
    jData['Width'] = width;
    jData['Height'] = height;
    if (error != null) {{
      jData['Error'] = error!.toJson();
    }}
    return jData;
  }}
}}");
        File.WriteAllText("output/models/serenity/web_response.dart", @$"import 'package:milow/models/api/json_factory.dart';
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
  int? entityId;
  dynamic customData;

  WebResponse(
      {{this.entities,
      this.entity,
      this.values,
      this.localizations,
      this.totalCount,
      this.skip,
      this.take,
      this.error,
      this.entityId,
      this.customData}});

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
    entityId = json['EntityId'];
    customData = json['CustomData'];
    if (json['Error'] != null) {{
      error = Error.fromJson(json['Error']);
    }}
  }}

  @override
  Map<String, dynamic> toJson() {{
    final Map<String, dynamic> jData = <String, dynamic>{{}};
    if (entities != null) {{
      jData['Entities'] = entities!.map((v) => v.toJson()).toList();
    }}
    jData['Values'] = values;
    jData['TotalCount'] = totalCount;
    jData['Skip'] = skip;
    jData['Take'] = take;
    jData['EntityId'] = entityId;
    jData['CustomData'] = customData;
    if (error != null) {{
      jData['Error'] = error!.toJson();
    }}
    return jData;
  }}
}}");
        File.WriteAllText("output/models/serenity/web_create_response.dart", @$"
import 'package:{Package}/models/api/json_factory.dart';
import 'package:{Package}/models/api/json_serializer.dart';
import 'package:{Package}/models/serenity/error.dart';

class WebCreateResponse<T extends JsonFactory> implements JsonFactory {{
  int? entityId;
  dynamic customData;
  Error? error;

  WebCreateResponse({{this.entityId, this.error, this.customData}});

  WebCreateResponse.fromJson(
      Map<String, dynamic> json, JsonSerializer<T> serializer) {{
    entityId = json['EntityId'];
    customData = json['CustomData'];
    if (json['Error'] != null) {{
      error = Error.fromJson(json['Error']);
    }}
  }}

  @override
  Map<String, dynamic> toJson() {{
    final Map<String, dynamic> jData = <String, dynamic>{{}};
    if (error != null) {{
      jData['Error'] = error!.toJson();
    }}
    jData['EntityId'] = entityId;
    jData['CustomData'] = customData;
    return jData;
  }}
}}
");
        #endregion


        #region services

        Directory.CreateDirectory("output/services");
        Directory.CreateDirectory("output/services/serenity");

        File.WriteAllText("output/services/serenity/serenity_service_factory.dart", $@"
import 'dart:io';

import 'package:{this.Package}/models/serenity/filter.dart';
import 'package:{this.Package}/models/serenity/web_file_response.dart';
import 'package:{this.Package}/models/serenity/web_response.dart';
import 'package:{this.Package}/models/api/json_factory.dart';

abstract class SerenityServiceFactory<T extends JsonFactory, TF extends Filter> {{
  Future<WebResponse<T>> list({{TF? filter}});
  Future<WebResponse<T>> delete(int entityId);
  Future<WebResponse<T>> retrieve(int entityId);
  Future<WebResponse<T>> create(T entity);
  Future<WebResponse<T>> update(int entityId, T entity);
  Future<WebFileResponse> upload(File file);
}}");
        File.WriteAllText("output/services/serenity/serenity_service.dart", $@"
import 'dart:io';
import 'dart:convert';
import 'dart:developer';

import 'package:dio/dio.dart';
import 'package:{this.Package}/environment.dart';
import 'package:{this.Package}/models/serenity/filter.dart';
import 'package:{this.Package}/models/serenity/web_response.dart';
import 'package:{this.Package}/models/serenity/web_file_response.dart';
import 'package:{this.Package}/models/api/json_serializer.dart';
import 'package:{this.Package}/models/api/json_factory.dart';
import 'package:{this.Package}/services/serenity/serenity_service_factory.dart';

final dioLoggerInterceptor = InterceptorsWrapper(onRequest: (RequestOptions options, handler) {{
  String headers = '';
  options.headers.forEach((key, value) {{
    headers += '| $key: $value';
  }});

  log('┌------------------------------------------------------------------------------');
  log('''| [DIO] Request: ${{options.method}} ${{options.uri}}
| ${{json.encode(options.data).toString()}}
| Headers:\n$headers''');
  log('├------------------------------------------------------------------------------');
  handler.next(options);  //continue
}}, onResponse: (Response response, handler) async {{
  log('| [DIO] Response [code ${{response.statusCode}}]: ${{json.encode(response.data).toString()}}');
  log('└------------------------------------------------------------------------------');
  handler.next(response);
  // return response; // continue
}}, onError: (DioError error, handler) async {{
  log('| [DIO] Error: ${{error.error}}: ${{error.response.toString()}}');
  log('└------------------------------------------------------------------------------');
  handler.next(error); //continue
}});

abstract class SerenityService<T extends JsonFactory, TF extends Filter> implements SerenityServiceFactory<T, TF> {{
  final Dio _dio = Dio(BaseOptions(
    baseUrl: API_DOMAIN,
    headers: {{
      'Authorization': API_AUTH,
      'Content-Type': 'application/json'
    }},
  ));

  late String apiName;
  late final TF defaultFilter;
  late JsonSerializer<T> jsonSerializer;

  SerenityService() {{
    _dio.interceptors.add(dioLoggerInterceptor);
  }}

  Future<WebResponse<T>> _makeCall(String url, dynamic data) async {{
    final result = await _dio.post(url, data: data);
    return WebResponse.fromJson(result.data, jsonSerializer);
  }}

  @override
  Future<WebFileResponse> upload(File file) async {{
    _dio.interceptors.clear();
    final fileName = file.path.split('/').last;
    final formData = FormData.fromMap({{
      'file': await MultipartFile.fromFile(file.path, filename: fileName),
    }});
    final result = await _dio.post('/File/TemporaryUpload', data: formData);
    return WebFileResponse.fromJson(json.decode(result.data));
    _dio.interceptors.add(dioLoggerInterceptor);
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
  Future<WebResponse<T>> update(int entityId, T entity) async {{
    return _makeCall(
        'Services/MilowDb/$apiName/Update', {{'EntityId': entityId, 'Entity' : entity}});
  }}

  @override
  Future<WebResponse<T>> create(T entity) async {{
    throw Exception('This should be overrided to call innerCreate');
  }}

  Future<WebResponse<T>> innerCreate(Map<String, dynamic> entity) async {{
    return _makeCall('Services/{this.Module}/$apiName/Create', {{'Entity': entity}});
  }}
}}");

        foreach (var entity in Db.Tables)
        {
            File.WriteAllText($"output/services/{entity.Name.Pathize()}_service.dart", $@"
import 'package:{this.Package}/models/{entity.Name.Pathize()}.dart';
import 'package:{this.Package}/models/api/json_serializer.dart';
import 'package:{this.Package}/models/filters/{entity.Name.Pathize()}_filter.dart';
import 'package:{this.Package}/services/serenity/serenity_service.dart';
import 'package:{this.Package}/services/serenity/serenity_service_factory.dart';
import 'package:{this.Package}/models/serenity/web_response.dart';

class {entity.Name.Singularize()}Service extends SerenityService<{entity.Name.Singularize()}, {entity.Name.Singularize()}Filter> implements {entity.Name.Singularize()}ServiceFactory {{
  {entity.Name.Singularize()}Service() {{
    apiName = '{entity.Name}';
    defaultFilter = {entity.Name.Singularize()}Filter(includeColumns: ['{string.Join("','", entity.Fields.Select(t => t.Name))}']);
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

abstract class {entity.Name.Singularize()}ServiceFactory extends SerenityServiceFactory<{entity.Name.Singularize()}, {entity.Name.Singularize()}Filter> {{}}

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