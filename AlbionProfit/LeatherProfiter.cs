using AlbionProfit.Game;
using AlbionProfit.Game.Items;
using AlbionProfit.Game.Market;
using AlbionProfit.Utility;

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
            if (prices.Length == 0)
            {
                continue;
            }

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
            if (prices.Length == 0)
            {
                continue;
            }
            
            _leathers.Add(prices[0].Item.Attributes, (prices[0].Item, prices));
        }
    }

    public Profit GetProfit(Attributes attributes)
    {
        if (!attributes.IsValid())
        {
            throw new Exception("Invalid attributes.");
        }

        (List<(Item item, float amount, bool isRefined)> neededItems, int totalFee) = GetDirectNeededItems(attributes);

        float lowestPrice = float.MaxValue;
        City lowestCity = City.Null;
        int lowestFee = 0;
        List<(Item item, float amount)> lowestItems = new List<(Item item, float amount)>();

        while (true)
        {
            foreach (City city in Enum.GetValues(typeof(City)))
            {
                if (city == City.Null)
                    continue;

                bool itemsExist = true;
                float price = 0;
                foreach ((Item item, float amount, bool isRefined) in neededItems)
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

                price += totalFee;

                if (!itemsExist)
                {
                    continue;
                }

                if (price < lowestPrice)
                {
                    lowestPrice = price;
                    lowestCity = city;
                    lowestFee = totalFee;
                    lowestItems = neededItems.Select(x => (x.item, x.amount)).ToList();
                }
            }

            List<(Item item, float amount, bool isRefined)> refinedNeededItems = neededItems.Where(neededItem => neededItem.isRefined).ToList();
            
            if (!refinedNeededItems.Any())
            {
                break;
            }
            
            foreach ((Item item, float amount, bool isRefined) neededItem in refinedNeededItems)
            {
                (List<(Item item, float amount, bool isRefined)> items, int craftingFee) = GetDirectNeededItems(neededItem.item.Attributes);
                totalFee += craftingFee;
                
                neededItems.Remove(neededItem);
                neededItems.AddRange(items);
            }
        }

        if (lowestCity == City.Null)
        {
            throw new Exception("Couldn't find lowest price.");
        } 
        
        float highestPrice = float.MinValue;
        City highestCity = City.Null;
        
        foreach (City city in Enum.GetValues(typeof(City)))
        {
            if(city == City.Null)
                continue;

            Price itemPrice = GetLeatherPriceInCity(attributes, city);
            if(itemPrice.City == City.Null)
                continue;

            float tax = Settings.Current.HasPremium ? 0.045f : 0.075f;
            
            float price = itemPrice.SellPrice - itemPrice.SellPrice * tax;

            if (price > highestPrice)
            {
                highestPrice = price;
                highestCity = city;
            }
        }

        if (highestCity == City.Null)
        {
            throw new Exception("Couldn't find highest price.");
        }

        return new Profit(_leathers[attributes].item, lowestPrice, lowestCity, highestPrice,  highestCity, lowestFee, lowestItems);
    }

    private Price GetHidePriceInCity(Attributes attributes, City city)
    {
        return _hides[attributes].prices.FirstOrDefault(x => x.City == city);
    }

    private Price GetLeatherPriceInCity(Attributes attributes, City city)
    {
        return _leathers[attributes].prices.FirstOrDefault(x => x.City == city);
    }

    private List<(Item item, float amount)> GetNeededItems(Attributes attributes)
    {
        List<(Item item, float amount)> neededItems = new List<(Item item, float amount)>();

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

        for (int i = 0; i < neededItems.Count; i++)
        {
            neededItems[i] = (neededItems[i].item, neededItems[i].amount - neededItems[i].amount * Settings.Current.ReturnRate / 100f);
        }

        return neededItems;
    }
    
    private (List<(Item item, float amount, bool isRefined)> items, int fee) GetDirectNeededItems(Attributes attributes)
    {
        List<(Item item, float amount, bool isRefined)> neededItems = new List<(Item item, float amount, bool isRefined)>();
        int fee = (int)MathF.Round(MathF.Pow(2, attributes.Tier - 3 + attributes.Enchantment) * 8 * 0.1125f * (Settings.Current.UsageFee / 100f));
        
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
        
        for (int i = 0; i < neededItems.Count; i++)
        {
             neededItems[i] = (neededItems[i].item, neededItems[i].amount - neededItems[i].amount * Settings.Current.ReturnRate / 100f, neededItems[i].isRefined);
        }

        return (neededItems, fee);
    }
}