
import './orders.dart';

class Shippers {
    int shipperid;
    String companyname;
    String? phone;

    List<Orders>? orders;

    

    Shippers({required this.shipperid, required this.companyname, this.phone, this.orders}){}
}