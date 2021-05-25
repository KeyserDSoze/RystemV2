namespace Rystem.Business
{
    public interface IData
    {
        string Name { get; }
        RystemDataServiceProvider ConfigureData();
        internal RystemDataServiceProvider BuildData()
            => ConfigureData().AddInstance(this.GetType());
    }
}