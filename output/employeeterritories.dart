
import './employees.dart';
import './territories.dart';

class EmployeeTerritories {
    int employeeid;
    String territoryid;

    

    Employees employees;
    Territories territories;

    EmployeeTerritories({required this.employeeid, required this.territoryid, required this.employees, required this.territories}){}
}