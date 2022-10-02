
import './products.dart';

class Categories {
    int categoryid;
    String categoryname;
    String? description;
    String? picture;

    List<Products>? products;

    

    Categories({required this.categoryid, required this.categoryname, this.description, this.picture, this.products}){}
}