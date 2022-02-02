using System.Globalization;
using Newtonsoft.Json;

namespace AlbionProfit;

public static class PriceChecker
{
    private const string PricePathPrefix = "https://www.albion-online-data.com/api/v2/stats/prices/";
    private const string PricePathSuffix = "?locations=Caerleon,Bridgewatch,Thetford,Fort%20sterling,Lymhurst,Martlock";

    private static readonly HttpClient Client = new HttpClient();

    public static async Task<Price[]> Check(Item item)
    {
        string response = await Client.GetStringAsync(PricePathPrefix + item.NameId + PricePathSuffix);
        ItemPriceJsonObject[] itemPrices = JsonConvert.DeserializeObject<ItemPriceJsonObject[]>(response);

        List<Price> prices = new List<Price>();
        for(int i = 0; i < itemPrices.Length; i++)
        {
            ItemPriceJsonObject currentPrice = itemPrices[i];

            if (currentPrice.BuyPriceMinDate == DateTime.Parse("0001-01-01T00:00:00"))
            {
                continue;
            }
            
            City city = StringToCity(currentPrice.City);
            if (city == City.Null)
            {
                continue;
            }

            if (currentPrice.Quality == 0)
            {
                continue;
            }
            Quality quality = IntToQuality(currentPrice.Quality);

            if (currentPrice.SellPriceMin == 0)
            {
                continue;
            }
            int sellPrice = currentPrice.SellPriceMin;

            prices.Add( new Price(item, city, quality, sellPrice));
        }

        return prices.ToArray();
    }

    private static City StringToCity(string str)
    {
        return str switch
        {
            "Caerleon" => City.Caerleon,
            "Thetford" => City.Thetford,
            "Bridgewatch" => City.Bridgewatch,
            "Fort Sterling" => City.FortSterling,
            "Martlock" => City.Martlock,
            "Lymhurst" => City.Lymhurst,
            _ => City.Null
        };
    }

    private static Quality IntToQuality(int i)
    {
        return i switch
        {
            1 => Quality.Normal,
            2 => Quality.Good,
            3 => Quality.Outstanding,
            4 => Quality.Excellent,
            5 => Quality.Masterpiece,
            _ => 0
        };
    }
    
    public class ItemPriceJsonObject
    {
        [JsonProperty("item_id")] 
        public string ItemId { get; set; } = string.Empty;

        [JsonProperty("city")]
        public string City { get; set; } = string.Empty;

        [JsonProperty("quality")]
        public int Quality { get; set; }

        [JsonProperty("sell_price_min")]
        public int SellPriceMin { get; set; }

        [JsonProperty("sell_price_min_date")]
        public DateTime SellPriceMinDate { get; set; }

        [JsonProperty("sell_price_max")]
        public int SellPriceMax { get; set; }

        [JsonProperty("sell_price_max_date")]
        public DateTime SellPriceMaxDate { get; set; }

        [JsonProperty("buy_price_min")]
        public int BuyPriceMin { get; set; }

        [JsonProperty("buy_price_min_date")]
        public DateTime BuyPriceMinDate { get; set; }

        [JsonProperty("buy_price_max")]
        public int BuyPriceMax { get; set; }

        [JsonProperty("buy_price_max_date")]
        public DateTime BuyPriceMaxDate { get; set; }
    }
}