namespace System
{
    public sealed class CsvProperty : Attribute
    {
        public string Name { get; }
        public CsvProperty(string name)
            => this.Name = name;
    }
}
