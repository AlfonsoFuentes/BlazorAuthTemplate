namespace Shared.Attributtes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BudgetMetadataAttribute : Attribute
    {
        public string Letter { get; }
        public string Name { get; }
        // Podrías agregar más cosas en el futuro, ej: public string HexColor { get; }

        public BudgetMetadataAttribute(string letter, string name)
        {
            Letter = letter;
            Name = name;
        }
    }
}
