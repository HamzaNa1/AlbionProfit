using System.Text;

namespace AlbionProfit
{
    public class ItemsRepository
    {
        private const string ItemsPath = "https://raw.githubusercontent.com/broderickhyman/ao-bin-dumps/master/formatted/items.txt";

        private readonly Dictionary<int, Item> _items;

        public ItemsRepository()
        {
            _items = new Dictionary<int, Item>();
        }

        public IEnumerable<Item> Search(string keyString)
        {
            return Search(keyString, SearchSettings.DefaultSettings);
        }
        
        public IEnumerable<Item> Search(string keyString, SearchSettings settings)
        {
            SearchKey key = CreateKey(keyString.ToLower());

            int amount = 0;
            foreach (Item item in _items.Values.Where(item => CompareKeyToItem(key, item, settings.ByName, settings.ByNameId)))
            {
                if (amount > settings.AmountOfResults)
                    break;
                yield return item;
                amount++;
            }
        }
        
        public IEnumerable<Item> Search(IEnumerable<string> keyStrings)
        {
            return Search(keyStrings, SearchSettings.DefaultSettings);
        }
        
        public IEnumerable<Item> Search(IEnumerable<string> keyStrings, SearchSettings settings)
        {
            List<Item> items = new List<Item>();
            
            foreach (string keyString in keyStrings)
            {
                items.AddRange(Search(keyString, settings));
            }

            return items;
        }

        public async Task LoadAllGameItems()
        {
            using HttpClient hc = new HttpClient();

            string data = await hc.GetStringAsync(ItemsPath);
            string[] itemsData = data.Split('\n');

            foreach (string itemData in itemsData)
            {
                if (string.IsNullOrWhiteSpace(itemData))
                {
                    continue;
                }

                Item item = CreateItem(itemData);
                _items.Add(item.Id, item);
            }
        } 

        private static bool CompareKeyToItem(SearchKey key, Item item, bool name = true, bool nameId = true)
        {
            bool text = key.Text.Count(x => name && item.Name.ToLower().Contains(x.ToLower()) || nameId && item.NameId.ToLower().Contains(x.ToLower())) == key.Text.Count();
            bool tier = key.Tier == -1 || key.Tier == item.Attributes.Tier;
            bool enchantment = key.Enchantment == -1 || key.Enchantment == item.Attributes.Enchantment;

            return text && tier && enchantment;
        }

        private static Item CreateItem(string itemData)
        {
            string[] splitItems = itemData.Split(':');
            
            int id = int.Parse(splitItems[0]);
            string nameId = FormatString(splitItems[1]);
            string name = splitItems.Length > 2 ? FormatString(splitItems[2]) : string.Empty;

            Item item = new Item(id, nameId, name);
            return item;
        }

        private static SearchKey CreateKey(string keyString)
        {
            string[] keys = keyString.Split(' ');

            List<string> text = new List<string>();
            int tier = -1;
            int enchantment = -1;

            foreach (string key in keys)
            {
                if (key.StartsWith("t") && key.Length is 2 or 4 && float.TryParse(key.Skip(1).Take(3).ToArray(), out float value))
                {
                    tier = (int)Math.Floor(value);
                    enchantment = (int)((value - tier) * 10);
                }
                else
                {
                    text.Add(key);
                }
            }

            SearchKey searchKey = new SearchKey(text, tier, enchantment);
            return searchKey;
        }

        private static string FormatString(string s)
        {
            StringBuilder newString = new StringBuilder();

            bool firstChar = s[0] != ' ';
            for (int i = 0; i < s.Length; i++)
            {
                if (!firstChar && s[i + 1] != ' ')
                {
                    firstChar = true;
                    continue;
                }

                if (s[i] == ' ' && (i == s.Length - 1 || s[i + 1] == ' '))
                {
                    break;
                }

                newString.Append(s[i]);
            }

            return newString.ToString();
        }
    }
}
