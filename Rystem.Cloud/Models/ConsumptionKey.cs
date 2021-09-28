namespace Rystem.Cloud
{
    public sealed record ConsumptionKey(string Category, string Subcategory, string Meter, string BillAccountId, string OfferId);
}