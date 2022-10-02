
import './region.dart';
import './employeeterritories.dart';

class Territories {
    String territoryid;
    String territorydescription;
    int regionid;

    List<EmployeeTerritories>? employeeterritories;

    Region region;

    Territories({required this.territoryid, required this.territorydescription, required this.regionid, required this.region, this.employeeterritories}){}
}