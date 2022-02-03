using AlbionProfit.Game.Items;

namespace AlbionProfit.Game.Market;

public readonly record struct Profit(Item Item, float BuyPrice, City BuyCity, float SellPrice, City SellCity, int fee, List<(Item item, float amount)> NeededItems);