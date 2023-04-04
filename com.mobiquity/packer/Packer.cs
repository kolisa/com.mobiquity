using com.mobiquity.exception;
using com.mobiquity.model;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
namespace com.mobiquity.packer;
public static class Packer
{
    public static string packs(string filePath)
    {

        try
        {
            StringBuilder result = new StringBuilder();
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }
            if (File.Exists(filePath))
            {

                // Open the text file using a stream reader.
                using (var sr = new StreamReader(filePath))
                {

                    List<string[]> lines = new List<string[]>();
                    string line;
                    // Read the stream as a string, and write the string to the console and split with :
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line.Split(':'));
                    }
                    //looping throug the rows of the package
                    foreach (string[] packageInfos in lines)
                    {

                        int limit = int.Parse(packageInfos[0].Trim());

                        if (limit < 100)
                        {

                            Bag bestBag = ProcessItems(packageInfos, limit);

                            FormatOutput(result, limit, bestBag);

                        }
                        else
                        {
                            throw new APIException("Package limit should be less than 100.");
                        }

                    }
                }

            }
            return result.ToString();
        }
        catch (FileNotFoundException)
        {
            throw new APIException("File doesn't exists. Path: " + filePath);
        }
    }
    /// <summary>
    /// Format output as required
    /// </summary>
    /// <param name="result"></param>
    /// <param name="limit"></param>
    /// <param name="bestBag"></param>
    private static void FormatOutput(StringBuilder result, int limit, Bag bestBag)
    {

        int[] arr = new int[bestBag.Items.Count()];
        int count = 0;

        if (bestBag.Items.Count() > 0)
        {

            bestBag.Items.ForEach(item =>
            {
                arr[count] = item.Index;
                count++;

            });
            count = 0;
            Array.Sort(arr);
            foreach (var item in arr)
            {
                if (count == 0)
                    result.Append(item);
                else result.Append("," + item);
                count++;

            }
            result.Append("\n");
        }
        else
        {
            result.Append("-\n");
        }
    }
    /// <summary>
    /// Process the item, when i will extract the items and find item to add to the bag
    /// </summary>
    /// <param name="packageInfos"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    private static Bag ProcessItems(string[] packageInfos, int limit)
    {
        string itemsInfos = packageInfos[1];

        string[]
        items = itemsInfos.Trim().Split(" ");

        Item[] itemsFromFile = ExtractItems(items);

        Bag bestBag = FindItem(itemsFromFile, limit);

        return bestBag;
    }
    /// <summary>
    /// Find the items that will be added on the bag and they must be less than or equal to the max wieght provided 
    /// </summary>
    /// <param name="items"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    private static Bag FindItem(Item[] items, int limit)
    {
        int NUMBERS_OF_ITEMS = items.Length;
        limit *= 100;
        // create two dimension array to keep the items
        int[,] matrix = new int[NUMBERS_OF_ITEMS + 1, limit + 1]; // set array size with limit and items
        for (int i = 0; i <= limit; i++)
        {
            matrix[0, i] = 0;
        }
        //looping through the item 
        for (int i = 1; i <= NUMBERS_OF_ITEMS; i++)
        {
            // loop through to check if its still less than the limit 
            for (int j = 0; j <= limit; j++)
            {

                int weightInt = (int)items[i - 1].Weight * 100;
                int priceInt = (int)items[i - 1].Price * 100;
                //add value to the array j less than limit value
                if (weightInt > j)
                {
                    matrix[i, j] = matrix[i - 1, j];
                }
                else
                {
                    // take the highest  values from the matrix added and add price
                    matrix[i, j] = Math.Max(matrix[i - 1, j], matrix[i - 1, j - weightInt] + priceInt);

                }
            }
        }

        int arrItem = matrix[NUMBERS_OF_ITEMS, limit];
        int weightLimit = limit;
        List<Item> itemsSolution = new List<Item>();
        //find item that will be added on the based  on the price and weight limits
        for (int i = NUMBERS_OF_ITEMS; i > 0 && arrItem > 0; i--)
        {
            if (arrItem != matrix[i - 1, weightLimit])
            {
                itemsSolution.Add(items[i - 1]);
                arrItem -= (int)items[i - 1].Price * 100;
                weightLimit -= (int)items[i - 1].Weight * 100;
            }
        }

        return new Bag(itemsSolution, (int)limit / 100);
    }
    /// <summary>
    /// Extract values from the line item and remove unwanted characters
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    /// <exception cref="APIException"></exception>
    private static Item[] ExtractItems(string[] items)
    {
        Item[]
        itemInfile = new Item[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            string[] infos = items[i].Replace("€", "").Replace("(", "").Replace(")", "").Split(",");

            try
            {
                int index = int.Parse(infos[0]);
                double weight = Double.Parse(infos[1]);
                double price = Double.Parse(infos[2]);

                Item item = new Item(index, weight, price);
                if (item.isValid())
                {
                    itemInfile[i] = item;
                }
            }
            catch (FormatException e)
            {
                throw new APIException("Format of number not accepted.");
            }


        }
        return itemInfile;
    }
}
