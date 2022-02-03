namespace AlbionProfit.Game.Market;

public struct SearchSettings
{
    public static readonly SearchSettings DefaultSettings = new SearchSettings();
    
    public int AmountOfResults { get; init; } = 10;
    public bool ByName { get; init; } = true;
    public bool ByNameId { get; init; } = true;
}