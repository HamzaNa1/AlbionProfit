namespace AlbionProfit
{
    public class Profiter
    {
        private readonly ItemsRepository _repository;
        private readonly SearchSettings _settings;

        public Profiter()
        {
            _repository = new ItemsRepository();
        }

        public Profiter(SearchSettings settings)
        {
            _settings = settings;
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
            for (int i = 4; i < 9; i++)
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
            
            string display = Displayer.GetDisplay(new[] { "Item", "City to buy at", "Buy for", "City to sell at", "Sell for", "Profit" },
                new[]
                {
                    orderedProfits.Select(x => x.Item.ToString()).ToArray(),
                    orderedProfits.Select(x => x.BuyCity.ToString()).ToArray(),
                    orderedProfits.Select(x => x.BuyPrice.ToString()).ToArray(),
                    orderedProfits.Select(x => x.SellCity.ToString()).ToArray(),
                    orderedProfits.Select(x => x.SellPrice.ToString()).ToArray(),
                    orderedProfits.Select(x =>
                    {
                        int profit = x.SellPrice - x.BuyPrice;
                        return (profit > 0 ? "+" : "") + profit;
                    }).ToArray()
                });
            
            Console.Clear();
            ExtraConsole.WriteLine(display);
        }
    }
}