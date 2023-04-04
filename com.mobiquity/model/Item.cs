
using com.mobiquity.exception;

namespace com.mobiquity.model;
public class Item
{
    public int Index { get; set; }
    public double Weight { get; set; }
    public double Price { get; set; }

    public Item(int _index, double _weight, double _price)
    {

        Index = _index;
        Weight = _weight;
        Price = _price;
    }

    public bool isValid()
    {
        if (Weight > 100)
        {
            throw new APIException("The item weight cannot be more than 100 max.");
        }

        if (Price > 100)
        {
            throw new APIException("The item weight cannot be more than 100 max.");
        }
        return true;
    }


}
