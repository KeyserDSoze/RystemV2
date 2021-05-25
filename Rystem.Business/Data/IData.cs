namespace Rystem.Business
{
    public interface IData
    {
        string Name { get; set; }
        RystemDataServiceProvider ConfigureData();
        internal RystemDataServiceProvider BuildData()
            => ConfigureData().AddInstance(this.GetType());
    }
}