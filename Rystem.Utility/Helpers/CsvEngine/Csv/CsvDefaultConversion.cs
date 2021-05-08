using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Rystem.Text
{
    internal class CsvDefaultConversion<TEntity>
        where TEntity : new()
    {
        private readonly char SplittingChar;
        private const string InCaseOfSplittedChar = "\"{0}\"";
        private const string InNormalCase = "{0}";
        private static readonly string BreakLine = '\n'.ToString();
        private readonly Regex SplittingRegex;
        private static readonly Type CsvIgnoreType = typeof(CsvIgnore);
        private static readonly Type CsvPropertyType = typeof(CsvProperty);
        private static readonly Dictionary<string, PropertyInfo> Properties = typeof(TEntity).GetProperties().Where(x => x.GetCustomAttribute(CsvIgnoreType) == default && StringablePrimitive.Check(x.PropertyType)).ToDictionary(x => x.GetCustomAttribute(CsvPropertyType) == default ? x.Name : ((CsvProperty)x.GetCustomAttribute(CsvPropertyType)).Name, x => x);
        private const string BaseRegex = @"(?<=^|{0})(\""(?:[^\""]|\""\"")*\""|[^{0}]*)";
        public CsvDefaultConversion(char splittingChar = ',')
        {
            this.SplittingChar = splittingChar;
            //this.SplittingRegex = new Regex($"(\\{this.SplittingChar}|\\r?\\n|\\r|^)(?:\"([^\"]*(?:\"\"[^\"] *) *)\"|([^\"\\{this.SplittingChar}\\r\\n]*))");
            this.SplittingRegex = new Regex(string.Format(BaseRegex, splittingChar.ToString()));
        }

        public string Write(IEnumerable<TEntity> entities)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string key in Properties.Keys)
                stringBuilder.Append($"{key}{this.SplittingChar}");
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append(BreakLine);
            foreach (TEntity entity in entities)
            {
                foreach (PropertyInfo propertyInfo in Properties.Values)
                {
                    string value = (propertyInfo.GetValue(entity) ?? string.Empty).ToString();
                    if (value.Contains(SplittingChar))
                        value = string.Format(InCaseOfSplittedChar, value.Replace("\"", "\"\""));
                    else
                        value = string.Format(InNormalCase, value);
                    stringBuilder.Append($"{value}{this.SplittingChar}");
                }
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
                stringBuilder.Append(BreakLine);
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }
        public IList<TEntity> Read(string value)
        {
            IList<TEntity> datas = new List<TEntity>();
            string[] values = value.Split('\n');
            MatchCollection properties = this.SplittingRegex.Matches(values.FirstOrDefault());
            foreach (string entry in values.Skip(1))
                datas.Add(FromString(entry, properties));
            return datas;
        }
        public IList<TEntity> Read(IEnumerable<string> values)
        {
            IList<TEntity> datas = new List<TEntity>();
            MatchCollection properties = this.SplittingRegex.Matches(values.FirstOrDefault());
            foreach (string entry in values.Skip(1))
                datas.Add(FromString(entry, properties));
            return datas;
        }
        private TEntity FromString(string value, MatchCollection properties)
        {
            TEntity data = new TEntity();
            MatchCollection values = this.SplittingRegex.Matches(value.Trim('\r'));
            int count = 0;
            foreach (Match property in properties)
            {
                if (count >= values.Count)
                    break;
                if (Properties.ContainsKey(property.Value))
                {
                    PropertyInfo propertyInfo = Properties[property.Value];
                    propertyInfo.SetValue(data, Convert.ChangeType(values[count].Value.Trim(SplittingChar), propertyInfo.PropertyType));
                }
                count++;
            }
            return data;
        }
    }
}
