public partial class Coder
{
    //
    // Summary:
    //   Write the DI file
    // Parameters:
    //   db:
    //     The read database
    public void DI()
    {
        System.IO.File.WriteAllText("output/di.dart", $@"
import 'package:flutter_simple_dependency_injection/injector.dart';
{string.Join("\n", this.Db.Tables.Select(t => $"import 'package:{this.Package}/services/{t.Name.Pathize()}_service.dart';"))}
class DependencyManager {{
  final _injector = Injector();
  get injector => _injector;

  static final DependencyManager _singleton = DependencyManager._();

  factory DependencyManager() {{
    return _singleton;
  }}

  DependencyManager._() {{
    _initialize();
  }}

  T get<T>() {{
    return injector.get<T>() as T;
  }}

  void _initialize() {{
    {string.Join("\n\t", this.Db.Tables.Select(t => $"injector.map<{t.Name.Singularize()}ServiceFactory>((i) => {t.Name.Singularize()}Service(),isSingleton: true);"))}
  }}
}}");
    }
}