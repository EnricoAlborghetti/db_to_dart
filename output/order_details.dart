
import './orders.dart';
import './products.dart';

class Order_Details {
    int orderid;
    int productid;
    double unitprice;
    int quantity;
    double discount;

    

    Orders orders;
    Products products;

    Order_Details({required this.orderid, required this.productid, required this.unitprice, required this.quantity, required this.discount, required this.orders, required this.products}){}
}