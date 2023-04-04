
namespace com.mobiquity.model;
public class Bag
{
    public List<Item> Items { get; set; }
    public int Limit { get; set; }

    public Bag(List<Item> _items, int _limit)
    {

        Items = _items;
        Limit = _limit;
    }

}
