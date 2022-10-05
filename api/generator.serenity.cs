public partial class Generator
{

    private String Package { get; set; }
    private String Module { get; set; }
    private Db Db { get; set; }

    //
    // Summary:
    //   Init the Generator with Serenity parameters
    // Parameters:
    //   db:
    //     the read database
    // Returns:
    //   the generator, can only use .Serenity()
    public static Generator BuildSerenity(Db db)
    {
        Console.WriteLine("Insert app package name [sample]");
        var package = Console.ReadLine().WithDefault("sample").Trim();

        Console.WriteLine($"Insert Serenity Module Name [{package}]");
        var serenityModule = Console.ReadLine().WithDefault(package).Trim();
        return new Generator(package, serenityModule, db);
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
    public Generator(string package, string module, Db db)
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
        File.WriteAllText("output/models/serenity/web_response.dart", @"
class Error {
  String code;
  String message;

  Error({required this.code, required this.message});
}

class WebResponse<T> {
  List<T>? entities;
  T? entity;
  dynamic values;
  dynamic localizations;
  int? totalCount;
  int? skip;
  int? take;
  Error? error;
  bool? cache;

  WebResponse(
      {this.entities,
      this.entity,
      this.values,
      this.localizations,
      this.totalCount,
      this.skip,
      this.take,
      this.error,
      this.cache});
}");
        #endregion


        #region services

        Directory.CreateDirectory("output/services");
        Directory.CreateDirectory("output/services/serenity");

        File.WriteAllText("output/services/serenity/serenity_service_factory.dart", $@"
import 'package:{this.Package}/models/serenity/filter.dart';
import 'package:{this.Package}/models/serenity/web_response.dart';

abstract class SerenityServiceFactory<T> {{
  Future<WebResponse<T>> list({{Filter? filter}});
  Future<WebResponse<T>> delete(int entityId);
  Future<WebResponse<T>> retrieve(int entityId);
  Future<WebResponse<T>> create(T entity);
}}");
        File.WriteAllText("output/services/serenity/serenity_service.dart", $@"
import 'package:dio/dio.dart';
import 'package:{this.Package}/models/serenity/filter.dart';
import 'package:{this.Package}/models/serenity/web_response.dart';
import 'package:{this.Package}/services/serenity/serenity_service_factory.dart';

abstract class SerenityService<T>
    implements SerenityServiceFactory {{
  final Dio _dio = Dio(BaseOptions(
    baseUrl: 'SET_BASE_URL',
    headers: {{
      'Authorization': 'SET_AUTH',
      'Content-Type': 'application/json'
    }},
  ));

  late String apiName;
  late Filter defaultFilter;

  SerenityService() {{
    _dio.interceptors.add(LogInterceptor(responseBody: true));
  }}

  Future<WebResponse<T>> _makeCall(String url, dynamic data) async {{
    final result = await _dio.post(url, data: data);
    return result as WebResponse<T>;
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
        'Services/{this.Module}/$apiName/Retrive', {{'EntityId': entityId}});
  }}

  @override
  Future<WebResponse<T>> create(dynamic entity) async {{
    return _makeCall('Services/{this.Module}/$apiName/Retrive', {{'Entity': entity}});
  }}
}}");

        foreach (var entity in Db.Tables)
        {
            File.WriteAllText($"output/services/{entity.Name.Pathize()}_service.dart", $@"
import 'package:{this.Package}/models/{entity.Name.Pathize()}.dart';
import 'package:{this.Package}/models/serenity/filter.dart';
import 'package:{this.Package}/services/serenity/serenity_service.dart';

class {entity.Name.Singularize()}Service extends SerenityService<{entity.Name.Singularize()}> implements {entity.Name.Singularize()}ServiceFactory {{
  {entity.Name.Singularize()}Service() {{
    apiName = '{entity.Name}';
    defaultFilter = Filter(take: 100, includeColumns: ['{string.Join("','", entity.Fields.Select(t => t.Name))}']);
  }}
}}

abstract class {entity.Name.Singularize()}ServiceFactory {{}}");
            Console.WriteLine($"Generated service for {entity.Name.Singularize()}");
        }

        #endregion
    }
}