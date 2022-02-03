using System.Globalization;

namespace AlbionProfit.Utility;

public class Settings
{
    public static readonly Settings Default = new Settings(4, 6, 0, 0, true, true, false);
    public static Settings Current = Default;

    public static int Amount => SettingsNames.Length;

    private static readonly string[] SettingsNames =
    {
        "Minimum Tier", "Maximum Tier", "Return Rate", "Usage Fee", "Buy Order Resources", "Sell Order Refined",
        "Has Premium"
    };

    public int MinTier { get; private set; }
    public int MaxTier { get; private set; }

    public float ReturnRate { get; private set; }
    public int UsageFee { get; private set; }
    public bool BuyOrderResources { get; private set; }
    public bool SellOrderRefined { get; private set; }
    public bool HasPremium { get; private set; }

    public Settings(int minTier, int maxTier, float returnRate, int usageFee, bool buyOrderResources,
        bool sellOrderRefined, bool hasPremium)
    {
        MinTier = minTier;
        MaxTier = maxTier;
        ReturnRate = returnRate;
        UsageFee = usageFee;
        BuyOrderResources = buyOrderResources;
        SellOrderRefined = sellOrderRefined;
        HasPremium = hasPremium;
    }

    public static void ChangeSetting(int index, int offset)
    {
        switch (index)
        {
            case 0:
                Current.MinTier += offset;
                if (Current.MinTier < 4)
                {
                    Current.MinTier = Current.MaxTier - 1;
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
                Current.ReturnRate = Displayer.GetValue("Return Rate: ");
                if (Current.ReturnRate > 100)
                {
                    Current.ReturnRate = 100;
                }
                else if (Current.ReturnRate < 0)
                {
                    Current.ReturnRate = 0;
                }

                break;
            case 3:
                Current.UsageFee = (int)Displayer.GetValue("Usage Fee: ");
                if (Current.UsageFee > 9999)
                {
                    Current.UsageFee = 9999;
                }
                else if (Current.UsageFee < 0)
                {
                    Current.UsageFee = 0;
                }

                break;
            case 4:
                Current.BuyOrderResources = !Current.BuyOrderResources;
                break;
            case 5:
                Current.SellOrderRefined = !Current.SellOrderRefined;
                break;
            case 6:
                Current.HasPremium = !Current.HasPremium;
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
            2 => Current.ReturnRate,
            3 => Current.UsageFee,
            4 => Current.BuyOrderResources,
            5 => Current.SellOrderRefined,
            6 => Current.HasPremium,
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

            float returnRate = float.Parse(settings[2]);
            int usageFee = int.Parse(settings[3]);

            bool buyOrderResources = settings[4] == "1";
            bool sellOrderRefined = settings[5] == "1";
            bool hasPremium = settings[6] == "1";

            Current = new Settings(minTier, maxTier, returnRate, usageFee, buyOrderResources, sellOrderRefined,
                hasPremium);
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
                    Current.MinTier.ToString(), Current.MaxTier.ToString(),
                    Current.ReturnRate.ToString(CultureInfo.InvariantCulture), Current.UsageFee.ToString(),
                    Current.BuyOrderResources ? "1" : "0",
                    Current.SellOrderRefined ? "1" : "0",
                    Current.HasPremium ? "1" : "0"
                });
        }
        catch
        {
            Console.WriteLine("Failed to save settings!");
        }
    }
}