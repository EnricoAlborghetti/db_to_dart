
import './territories.dart';

class Region {
    int regionid;
    String regiondescription;

    List<Territories>? territories;

    

    Region({required this.regionid, required this.regiondescription, this.territories}){}
}