
import './suppliers.dart';
import './categories.dart';
import './order_details.dart';

class Products {
    int productid;
    String productname;
    int? supplierid;
    int? categoryid;
    String? quantityperunit;
    double? unitprice;
    int? unitsinstock;
    int? unitsonorder;
    int? reorderlevel;
    bool discontinued;

    List<Order_Details>? order_details;

    Suppliers? suppliers;
    Categories? categories;

    Products({required this.productid, required this.productname, this.supplierid, this.categoryid, this.quantityperunit, this.unitprice, this.unitsinstock, this.unitsonorder, this.reorderlevel, required this.discontinued, this.suppliers, this.categories, this.order_details}){}
}