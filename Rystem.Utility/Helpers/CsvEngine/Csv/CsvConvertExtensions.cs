namespace Rystem.Text
{
    internal static class CsvConvertExtensions
    {
        public static string ToObjectCsv<T>(this T data)
           => new StartConversion().Serialize(data);
        public static T FromObjectCsv<T>(this string value)
           => (T)new StartConversion().Deserialize(typeof(T), value);
    }
}
