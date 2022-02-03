using System.Text;

namespace AlbionProfit.Utility;

public static class Displayer
{
    public static string GetDisplay(string[] categories, string[][] values)
    {
        int[] sizes = new int[categories.Length];
        for (int i = 0; i < sizes.Length; i++)
        {
            int max = categories[i].Length;

            for (int j = 0; j < values[i].Length; j++)
            {
                if (values[i][j].Length > max)
                {
                    max = values[i][j].Length;
                }
            }

            sizes[i] = max;
        }

        StringBuilder sb = new StringBuilder();

        sb.Append("┌─");
        for (int i = 0; i < sizes.Length; i++)
        {
            sb.Append(new string('─', sizes[i]));
            if (i != categories.Length - 1)
            {
                sb.Append("─┬─");
            }
        }

        sb.Append("─┐");
        sb.Append(Environment.NewLine);

        sb.Append("│ ");
        for (int i = 0; i < categories.Length; i++)
        {
            sb.Append(categories[i]);
            sb.Append(new string(' ', sizes[i] - categories[i].Length));

            if (i != categories.Length - 1)
            {
                sb.Append(" │ ");
            }
        }

        sb.Append(" │");

        sb.Append(Environment.NewLine);
        sb.Append("├─");
        for (int i = 0; i < sizes.Length; i++)
        {
            sb.Append(new string('─', sizes[i]));
            if (i != categories.Length - 1)
            {
                sb.Append("─┼─");
            }
        }

        sb.Append("─┤");
        sb.Append(Environment.NewLine);

        for (int i = 0; i < values[0].Length; i++)
        {
            sb.Append("│ ");
            for (int j = 0; j < values.Length; j++)
            {
                if (values[j][i].StartsWith("+"))
                {
                    sb.Append("<f=green>");
                    sb.Append(values[j][i]);
                    sb.Append("<f=gray>");
                }
                else if (values[j][i].StartsWith("-"))
                {
                    sb.Append("<f=red>");
                    sb.Append(values[j][i]);
                    sb.Append("<f=gray>");
                }
                else
                {
                    sb.Append(values[j][i]);
                }

                sb.Append(new string(' ', sizes[j] - values[j][i].Length));

                if (j != values.Length - 1)
                {
                    sb.Append(" │ ");
                }
            }

            sb.Append(" │");
            sb.Append(Environment.NewLine);
        }

        sb.Append("└─");
        for (int i = 0; i < sizes.Length; i++)
        {
            sb.Append(new string('─', sizes[i]));
            if (i != categories.Length - 1)
            {
                sb.Append("─┴─");
            }
        }

        sb.Append("─┘");

        return sb.ToString();
    }
    
    public static void SettingsMenu()
    {
        int index = 0;
        bool escape = false;
        
        StringBuilder sb = new StringBuilder();
        while (true)
        {
            Console.Clear();
            sb.Clear();            
            for (int i = 0; i < Settings.Amount; i++)
            {
                if (index == i)
                {
                    sb.Append("<f=green>");
                    sb.Append(Settings.GetSetting(i));
                    sb.Append(" >");
                }
                else
                {
                    sb.Append("<f=gray>");
                    sb.Append(Settings.GetSetting(i));
                }

                sb.AppendLine();
            }
            sb.Append("<f=gray>");
            sb.AppendLine("<f=red>Use the up and down arrows to move and the left and right arrows to change the values");
            sb.AppendLine("<f=red>Press escape to go back...");
            ExtraConsole.Write(sb.ToString());

            ConsoleKeyInfo key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                {
                    index--;
                    if (index < 0)
                    {
                        index = Settings.Amount - 1;
                    }

                    break;
                }
                case ConsoleKey.DownArrow:
                {
                    index++;
                    if (index >= Settings.Amount)
                    {
                        index = 0;
                    }

                    break;
                }
                case ConsoleKey.RightArrow:
                    Settings.ChangeSetting(index, 1);
                    break;
                case ConsoleKey.LeftArrow:
                    Settings.ChangeSetting(index, -1);
                    break;
                case ConsoleKey.Escape:
                    escape = true;
                    break;
            }

            if (escape)
            {
                break;
            }
        }
    }
    
    public static float GetValue(string message)
    {
        float value = 0;
        string? input;
        do
        {
            Console.Clear();
            Console.Write(message);
            input = Console.ReadLine();
        } while (input is null || !float.TryParse(input, out value));

        return value;
    }
}