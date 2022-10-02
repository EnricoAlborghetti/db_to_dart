
import './products.dart';

class Suppliers {
    int supplierid;
    String companyname;
    String? contactname;
    String? contacttitle;
    String? address;
    String? city;
    String? region;
    String? postalcode;
    String? country;
    String? phone;
    String? fax;
    String? homepage;

    List<Products>? products;

    

    Suppliers({required this.supplierid, required this.companyname, this.contactname, this.contacttitle, this.address, this.city, this.region, this.postalcode, this.country, this.phone, this.fax, this.homepage, this.products}){}
}