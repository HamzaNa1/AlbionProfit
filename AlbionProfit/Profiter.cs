using System.Globalization;
using AlbionProfit.Game.Items;
using AlbionProfit.Game.Market;
using AlbionProfit.Utility;

namespace AlbionProfit
{
    public class Profiter
    {
        private readonly ItemsRepository _repository;

        public Profiter()
        {
            _repository = new ItemsRepository();
        }

        public async Task Initialize()
        {
            Console.WriteLine("Loading all items...");
            await _repository.LoadAllGameItems();
        }
        
        public void GetProfit(RefinedResource choice)
        {
            Console.WriteLine("Getting materials...");
            IEnumerable<Item> resource = _repository.Search(CommonItems.Resources[choice]);
            IEnumerable<Item> refinedResource = _repository.Search(CommonItems.RefinedResources[choice]);

            Console.WriteLine("Initializing profiter...");
            LeatherProfiter leatherProfiter = new LeatherProfiter(resource, refinedResource);

            Console.WriteLine("Getting profit...");
            List<Profit> profits = new List<Profit>();
            for (int i = Settings.Current.MinTier; i < Settings.Current.MaxTier + 1; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    try
                    {
                        profits.Add(leatherProfiter.GetProfit(new Attributes(i, j)));
                    }
                    catch
                    {
                        Console.WriteLine($"Couldn't find materials of T{i}.{j}");
                    }
                }
            }

            List<Profit> orderedProfits = profits.OrderByDescending(x => x.SellPrice - x.BuyPrice).ToList();
            DisplayProfit(orderedProfits);
        }

        private static void DisplayProfit(IReadOnlyCollection<Profit> profits)
        {
            bool plan = false;
            int number = 0;

            do
            {
                string display = !plan? Displayer.GetDisplay(
                    new[] { "N", "Item", "City to buy at", "Buy for", "City to sell at", "Sell for", "Profit" },
                    new[]
                    {
                        Enumerable.Range(1, profits.Count).Select(x => x.ToString()).ToArray(),
                        profits.Select(x => x.Item.ToString()).ToArray(),
                        profits.Select(x => x.BuyCity.ToString()).ToArray(),
                        profits.Select(x => x.BuyPrice.ToString("0.00")).ToArray(),
                        profits.Select(x => x.SellCity.ToString()).ToArray(),
                        profits.Select(x => x.SellPrice.ToString("0.00")).ToArray(),
                        profits.Select(x =>
                        {
                            float profit = x.SellPrice - x.BuyPrice;
                            return (profit > 0 ? "+" : "") + profit.ToString("0.00");
                        }).ToArray()
                    }) :
                        $"Plan for {profits.ElementAt(number - 1).Item}\nCrafting Fee:{profits.ElementAt(number - 1).fee}\n" + Displayer.GetDisplay(new [] {"Item", "Quantity"}, new []
                        {
                            profits.ElementAt(number - 1).NeededItems.Select(x => x.item.ToString()).ToArray(),
                            profits.ElementAt(number - 1).NeededItems.Select(x => x.amount.ToString("0.00")).ToArray()
                        });

                Console.Clear();
                ExtraConsole.WriteLine(display);
                Console.WriteLine(plan
                    ? "Press any key to go back..."
                    : "Type the number of the resource to get the plan or (esc) to go back...");
                if (plan)
                {
                    Console.ReadKey();
                    plan = false;
                }
                else
                {
                    string? command = Console.ReadLine();

                    if (command is not null)
                    {
                        if (command == "esc")
                        {
                            break;
                        }
                        else if (int.TryParse(command, out number))
                        {
                            if (number > 0 && number <= profits.Count)
                            {
                                plan = true;
                            }
                        }
                    }
                }

            } while (true);
        }
    }
}