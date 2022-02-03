using AlbionProfit;
using AlbionProfit.Game.Items;
using AlbionProfit.Utility;

Console.Clear();
Settings.LoadSettings();

Profiter profiter = new Profiter();
await profiter.Initialize();

string? currentVersion = VersionChecker.GetCurrentVersion();
string apiVersion = await VersionChecker.GetVersionFromApi();
bool isUpToDate = currentVersion == apiVersion;

while (true)
{
    string? selected;
    int choice;
    
    do
    {
        Console.Clear();
        ExtraConsole.WriteLine("<f=green>AlbionProfit v" + currentVersion + "<f=gray>");
        Console.WriteLine("Please Select a Resource:");
        Console.WriteLine("1. Leather");
        Console.WriteLine("2. Cloth");
        Console.WriteLine("3. Planks");
        Console.WriteLine("4. Metal Bar");
        Console.WriteLine("or type 5 to change settings");
        if (!isUpToDate)
        {
            ExtraConsole.WriteLine($"<f=red>There's a new version ({apiVersion}), Please update using (dotnet tool update --global AlbionProfit) and restart the app!<f=gray>");
        }
        // Console.WriteLine("5. Stone Blocks");
        selected = Console.ReadLine();
    } while (selected is null || !int.TryParse(selected, out choice) || choice is < 1 or > 5);

    Console.Clear();

    if (choice == 5)
    {
        Displayer.SettingsMenu();
    }
    else
    {
        profiter.GetProfit(((RefinedResource[])Enum.GetValues(typeof(RefinedResource)))[choice - 1]);
    }
}
