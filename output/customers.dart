
import './orders.dart';

class Customers {
    String customerid;
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

    List<Orders>? orders;

    

    Customers({required this.customerid, required this.companyname, this.contactname, this.contacttitle, this.address, this.city, this.region, this.postalcode, this.country, this.phone, this.fax, this.orders}){}
}