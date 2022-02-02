using System.Text;

namespace AlbionProfit;

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
}