using AlbionProfit.Game.Items;

namespace AlbionProfit.Game.Market;

public readonly record struct Price(Item Item, City City, Quality Quality, int SellPrice);