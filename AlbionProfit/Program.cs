using AlbionProfit;

Profiter profiter = new Profiter();
await profiter.Initialize();

while (true)
{
    string? selected;
    int choice;
    
    do
    {
        Console.Clear();
        Console.WriteLine("Please Select a Resource:");
        Console.WriteLine("1. Leather");
        Console.WriteLine("2. Cloth");
        Console.WriteLine("3. Planks");
        Console.WriteLine("4. Metal Bar");
        // Console.WriteLine("5. Stone Blocks");
        selected = Console.ReadLine();
    } while (selected is null || !int.TryParse(selected, out choice) || choice is < 1 or > 4);

    Console.Clear();

    profiter.GetProfit(((RefinedResource[])Enum.GetValues(typeof(RefinedResource)))[choice - 1]);

    Console.WriteLine("Press any key to go back...");
    Console.ReadKey();
}
