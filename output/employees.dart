
import './employees.dart';
import './orders.dart';

class Employees {
    int employeeid;
    String lastname;
    String firstname;
    String? title;
    String? titleofcourtesy;
    DateTime? birthdate;
    DateTime? hiredate;
    String? address;
    String? city;
    String? region;
    String? postalcode;
    String? country;
    String? homephone;
    String? extension;
    String? photo;
    String? notes;
    int? reportsto;
    String? photopath;

    List<Orders>? orders;

    Employees? employees;

    Employees({required this.employeeid, required this.lastname, required this.firstname, this.title, this.titleofcourtesy, this.birthdate, this.hiredate, this.address, this.city, this.region, this.postalcode, this.country, this.homephone, this.extension, this.photo, this.notes, this.reportsto, this.photopath, this.employees, this.orders}){}
}