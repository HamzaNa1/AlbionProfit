namespace AlbionProfit;

public class LeatherProfiter
{
    private readonly Dictionary<Attributes, (Item item, Price[] prices)> _hides;
    private readonly Dictionary<Attributes, (Item item, Price[] prices)> _leathers;

    public LeatherProfiter(IEnumerable<Item> hides, IEnumerable<Item> leathers)
    {
        _hides = new Dictionary<Attributes, (Item item, Price[] prices)>();
        _leathers = new Dictionary<Attributes, (Item item, Price[] prices)>();

        List<Task<Price[]>> hideTasks = hides.Select(x => PriceChecker.Check(x, Settings.Current.BuyOrderResources)).ToList();
        List<Task<Price[]>> leatherTasks = leathers.Select(x => PriceChecker.Check(x, !Settings.Current.SellOrderRefined)).ToList();

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

        List<(Item item, int amount, bool isRefined)> neededItems = GetDirectNeededItems(attributes);

        int lowestPrice = int.MaxValue;
        City lowestCity = City.Null;
        List<(Item item, int amount)> lowestItems = new List<(Item item, int amount)>();

        while (true)
        {
            foreach (City city in Enum.GetValues(typeof(City)))
            {
                if (city == City.Null)
                    continue;

                bool itemsExist = true;
                int price = 0;
                foreach ((Item item, int amount, bool isRefined) in neededItems)
                {
                    Price itemPrice = isRefined
                        ? GetLeatherPriceInCity(item.Attributes, city)
                        : GetHidePriceInCity(item.Attributes, city);

                    if (itemPrice.City == City.Null)
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
                    lowestItems = neededItems.Select(x => (x.item, x.amount)).ToList();
                }
            }

            bool brk = true;
            foreach ((Item item, int amount, bool isRefined) neededItem in neededItems.ToList())
            {
                if (neededItem.isRefined)
                {
                    neededItems.Remove(neededItem);
                    neededItems.AddRange(GetDirectNeededItems(neededItem.item.Attributes));
                    brk = false;
                }
            }

            if (brk)
            {
                break;
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

        return new Profit(_leathers[attributes].item, lowestPrice, lowestCity, highestPrice, highestCity, lowestItems);
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
    
    private List<(Item item, int amount, bool isRefined)> GetDirectNeededItems(Attributes attributes)
    {
        List<(Item item, int amount, bool isRefined)> neededItems = new List<(Item item, int amount, bool isRefined)>();

        switch (attributes.Tier)
        {
            case >= 5:
            {
                neededItems.Add((_leathers[new Attributes(attributes.Tier - 1, attributes.Enchantment)].item, 1, true));

                int quantity = attributes.Tier switch
                {
                    >= 7 => 5,
                    >= 6 => 4,
                    >= 5 => 3,
                    _ => 1
                };

                neededItems.Add((_hides[new Attributes(attributes.Tier, attributes.Enchantment)].item, quantity, false));
                break;
            }
            case 4:
                neededItems.Add((_leathers[new Attributes(attributes.Tier - 1, 0)].item, 1, true));
                neededItems.Add((_hides[new Attributes(attributes.Tier, attributes.Enchantment)].item, 2, false));
                break;
            case 3:
                neededItems.Add((_hides[new Attributes(3, 0)].item, 2, false));
                neededItems.Add((_hides[new Attributes(2, 0)].item, 1, false));
                break;
            case 2:
                neededItems.Add((_hides[new Attributes(2, 0)].item, 1, false));
                break;
        }

        return neededItems;
    }
}