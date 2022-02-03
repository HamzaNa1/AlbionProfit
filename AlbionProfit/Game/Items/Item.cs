using System.Text;

namespace AlbionProfit.Game.Items
{
    public readonly struct Item
    {
        public int Id { get; }
        public string NameId { get; }
        public string Name { get; }

        public Attributes Attributes { get; }

        public Item(int id, string nameId, string name)
        {
            Id = id;
            NameId = nameId;
            Name = name;

            int tier = 0;
            int enchantment = 0;

            if (NameId[0] == 'T' && NameId[2] == '_')
            {
                bool _ = int.TryParse(NameId[1].ToString(), out tier);
            }


            if (NameId[^2] == '@')
            {
                bool _ = int.TryParse(NameId[^1].ToString(), out enchantment);
            }

            Attributes = new Attributes(tier, enchantment);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            stringBuilder.Append(Name);
            if (Attributes.Tier > 0)
            {
                stringBuilder.Append(" T");
                stringBuilder.Append(Attributes.Tier);

                if (Attributes.Enchantment > 0)
                {
                    stringBuilder.Append('.');
                    stringBuilder.Append(Attributes.Enchantment);
                }
            }

            return stringBuilder.ToString();
        }
    }
}