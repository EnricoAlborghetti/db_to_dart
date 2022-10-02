
import './customers.dart';
import './employees.dart';
import './shippers.dart';
import './order_details.dart';

class Orders {
    int orderid;
    String? customerid;
    int? employeeid;
    DateTime? orderdate;
    DateTime? requireddate;
    DateTime? shippeddate;
    int? shipvia;
    double? freight;
    String? shipname;
    String? shipaddress;
    String? shipcity;
    String? shipregion;
    String? shippostalcode;
    String? shipcountry;

    List<Order_Details>? order_details;

    Customers? customers;
    Employees? employees;
    Shippers? shippers;

    Orders({required this.orderid, this.customerid, this.employeeid, this.orderdate, this.requireddate, this.shippeddate, this.shipvia, this.freight, this.shipname, this.shipaddress, this.shipcity, this.shipregion, this.shippostalcode, this.shipcountry, this.customers, this.employees, this.shippers, this.order_details}){}
}