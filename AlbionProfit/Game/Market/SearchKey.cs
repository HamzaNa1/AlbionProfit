namespace AlbionProfit.Game.Market
{
    public readonly struct SearchKey
    {
        public IEnumerable<string> Text { get; }
        public int Tier { get; }
        public int Enchantment { get; }

        public SearchKey(IEnumerable<string> text, int tier, int enchantment)
        {
            Text = text;
            Tier = tier;
            Enchantment = enchantment;
        }
    }
}
