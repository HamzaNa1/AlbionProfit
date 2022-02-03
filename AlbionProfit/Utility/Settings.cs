namespace AlbionProfit.Utility;

public class Settings
{
    public static readonly Settings Default = new Settings(4, 6, true, true);
    public static Settings Current = Default;
    
    private static readonly string[] SettingsNames = { "Minimum Tier", "Maximum Tier", "Buy Order Resources", "Sell Order Refined" };

    public int MinTier { get; private set; }
    public int MaxTier { get; private set; }
    public bool BuyOrderResources { get; private set; }
    public bool SellOrderRefined { get; private set; }
    
    public Settings(int minTier, int maxTier, bool buyOrderResources, bool sellOrderRefined)
    {
        MinTier = minTier;
        MaxTier = maxTier;
        BuyOrderResources = buyOrderResources;
        SellOrderRefined = sellOrderRefined;
    }
    
    public static void ChangeSetting(int index, int offset)
    {
        switch (index)
        {
            case 0:
                Current.MinTier += offset;
                if (Current.MinTier < 4)
                {
                    Current.MinTier = Current.MaxTier-1;
                }
                else if (Current.MinTier > 8 || Current.MinTier >= Current.MaxTier)
                {
                    Current.MinTier = 4;
                }
                break;
            case 1:
                Current.MaxTier += offset;
                if (Current.MaxTier < 4 || Current.MaxTier <= Current.MinTier)
                {
                    Current.MaxTier = 8;
                }
                else if (Current.MaxTier > 8)
                {
                    Current.MaxTier = Current.MinTier + 1;
                }
                break;
            case 2:
                Current.BuyOrderResources = !Current.BuyOrderResources;
                break;
            case 3:
                Current.SellOrderRefined = !Current.SellOrderRefined;
                break;
        }
        
        SaveCurrentSettings();
    }

    public static string GetSetting(int index)
    {
        return SettingsNames[index] + ": " + index switch
        {
            0 => Current.MinTier,
            1 => Current.MaxTier,
            2 => Current.BuyOrderResources,
            3 => Current.SellOrderRefined,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };
    }
    
    public static void LoadSettings()
    {
        if (!File.Exists("settings.txt"))
        {
            return;
        }

        try
        {
            string[] settings = File.ReadAllLines("settings.txt");

            int minTier = int.Parse(settings[0]);
            int maxTier = int.Parse(settings[1]);
            bool buyOrderResources = settings[2] == "1";
            bool sellOrderRefined = settings[3] == "1";

            Current = new Settings(minTier, maxTier, buyOrderResources, sellOrderRefined);
        }
        catch
        {
            Console.WriteLine("Failed to load settings!");
        }
    }

    public static void SaveCurrentSettings()
    {
        try
        {
            File.WriteAllLines("settings.txt",
                new[]
                {
                    Current.MinTier.ToString(), Current.MaxTier.ToString(), Current.BuyOrderResources ? "1" : "0",
                    Current.SellOrderRefined ? "1" : "0"
                });
        }
        catch
        {
            Console.WriteLine("Failed to save settings!");
        }
    }
}