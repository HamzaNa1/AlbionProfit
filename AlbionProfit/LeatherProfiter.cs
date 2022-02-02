namespace AlbionProfit;

public class LeatherProfiter
{
    private readonly Dictionary<Attributes, (Item item, Price[] prices)> _hides;
    private readonly Dictionary<Attributes, (Item item, Price[] prices)> _leathers;

    public LeatherProfiter(IEnumerable<Item> hides, IEnumerable<Item> leathers)
    {
        _hides = new Dictionary<Attributes, (Item item, Price[] prices)>();
        _leathers = new Dictionary<Attributes, (Item item, Price[] prices)>();

        List<Task<Price[]>> hideTasks = hides.Select(PriceChecker.Check).ToList();
        List<Task<Price[]>> leatherTasks = leathers.Select(PriceChecker.Check).ToList();

        foreach (Task<Price[]> task in hideTasks)
        {
            task.Wait();
            if (task.Exception is not null)
            {
                throw task.Exception;
            }

            Price[] prices = task.Result;
            _hides.Add(prices[0].Item.Attributes, (prices[0].Item, prices));
        }

        foreach (Task<Price[]> task in leatherTasks)
        {
            task.Wait();
            if (task.Exception is not null)
            {
                throw task.Exception;
            }

            Price[] prices = task.Result;
            _leathers.Add(prices[0].Item.Attributes, (prices[0].Item, prices));
        }
    }

    public Profit GetProfit(Attributes attributes)
    {
        if (!attributes.IsValid())
        {
            throw new Exception("Invalid attributes.");
        }

        List<(Item item, int amount)> neededItems = GetNeededItems(attributes);

        int lowestPrice = int.MaxValue;
        City lowestCity = City.Null;
        
        foreach (City city in Enum.GetValues(typeof(City)))
        {
            if(city == City.Null)
                continue;

            bool itemsExist = true;
            int price = 0;
            foreach ((Item item, int amount) in neededItems)
            {
                Price itemPrice = GetHidePriceInCity(item.Attributes, city);
                if(itemPrice.City == City.Null)
                {
                    itemsExist = false;    
                    break;
                }        
                
                price += itemPrice.SellPrice * amount;
            }

            if (!itemsExist)
            {
                continue;
            }

            int average = price / neededItems.Count;
            if (average < lowestPrice)
            {
                lowestPrice = average;
                lowestCity = city;
            }
        }

        if (lowestCity == City.Null)
        {
            throw new Exception("Couldn't find lowest price.");
        }
        
        int highestPrice = int.MinValue;
        City highestCity = City.Null;
        
        foreach (City city in Enum.GetValues(typeof(City)))
        {
            if(city == City.Null)
                continue;

            Price itemPrice = GetLeatherPriceInCity(attributes, city);
            if(itemPrice.City == City.Null)
                continue;
            
            int price = itemPrice.SellPrice;

            int average = price / neededItems.Count;
            if (average > highestPrice)
            {
                highestPrice = average;
                highestCity = city;
            }
        }

        if (highestCity == City.Null)
        {
            throw new Exception("Couldn't find highest price.");
        }

        return new Profit(_leathers[attributes].item, lowestPrice, lowestCity, highestPrice, highestCity);
    }

    private Price GetHidePriceInCity(Attributes attributes, City city)
    {
        return _hides[attributes].prices.FirstOrDefault(x => x.City == city);
    }

    private Price GetLeatherPriceInCity(Attributes attributes, City city)
    {
        return _leathers[attributes].prices.FirstOrDefault(x => x.City == city);
    }

    private List<(Item item, int amount)> GetNeededItems(Attributes attributes)
    {
        List<(Item item, int amount)> neededItems = new List<(Item item, int amount)>();

        switch (attributes.Tier)
        {
            case >= 5:
            {
                neededItems.AddRange(GetNeededItems(new Attributes(attributes.Tier - 1, attributes.Enchantment)));

                int quantity = attributes.Tier switch
                {
                    >= 7 => 5,
                    >= 6 => 4,
                    >= 5 => 3,
                    _ => 1
                };

                neededItems.Add((_hides[new Attributes(attributes.Tier, attributes.Enchantment)].item, quantity));
                break;
            }
            case 4:
                neededItems.AddRange(GetNeededItems(new Attributes(attributes.Tier - 1, 0)));
                neededItems.Add((_hides[new Attributes(attributes.Tier, attributes.Enchantment)].item, 2));
                break;
            case 3:
                neededItems.Add((_hides[new Attributes(3, 0)].item, 2));
                neededItems.Add((_hides[new Attributes(2, 0)].item, 1));
                break;
            case 2:
                neededItems.Add((_hides[new Attributes(2, 0)].item, 1));
                break;
        }

        return neededItems;
    }
}