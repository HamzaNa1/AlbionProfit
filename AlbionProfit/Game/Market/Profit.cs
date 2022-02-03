using AlbionProfit.Game.Items;

namespace AlbionProfit.Game.Market;

public readonly record struct Profit(Item Item, int BuyPrice, City BuyCity, int SellPrice, City SellCity, List<(Item item, int amount)> NeededItems);