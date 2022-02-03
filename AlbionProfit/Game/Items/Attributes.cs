namespace AlbionProfit.Game.Items;

public readonly struct Attributes
{
    public int Tier { get; }
    public int Enchantment { get; }

    public Attributes(int tier, int enchantment)
    {
        Tier = tier;
        Enchantment = enchantment;
    }

    public bool IsValid()
    {
        if (Enchantment > 0 && Tier < 4)
        {
            return false;
        }

        if (Tier is > 8 or < 0)
        {
            return false;
        }

        if (Enchantment is < 0 or > 3)
        {
            return false;
        }

        return true;
    }
}