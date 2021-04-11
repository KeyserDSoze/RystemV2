using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Rystem.UnitTest
{
    public class Csv
    {
        [Fact]
        public async Task IsOk()
        {
            List<CsvModel> csvs = new List<CsvModel>();
            for (int i = 0; i < 100; i++)
                csvs.Add(new CsvModel()
                {
                    Name = "Ale",
                    Babel = "dsjakld,dsjakdljsa\",\"dsakdljsa\",dsadksa;dl,\",\",",
                    Hotel = i,
                    Value = 33D,
                    Nothing = 34
                });
            List<CsvModel2> csvs2 = new List<CsvModel2>();
            for (int i = 0; i < 100; i++)
                csvs2.Add(new CsvModel2()
                {
                    Name = "Alddde",
                    Babel = "dsajakld|d\"sjakdljsa,\"||\"||sakdljsa\",dsadksa;dl||.fd,",
                    Hotel = i,
                    Value = 33D,
                    Nothing = 34
                });
            string firstCsv = csvs.ToCsv();
            string secondCsv = csvs2.ToCsv('|');
            Assert.Contains("Corto", firstCsv);
            List<CsvModel> csvsComparer = firstCsv.FromCsv<CsvModel>().ToList();
            List<CsvModel2> csvs2Comparer = secondCsv.FromCsv<CsvModel2>('|').ToList();
            bool check = true;
            for (int i = 0; i < 100; i++)
            {
                check = csvs[i].Hotel == csvsComparer[i].Hotel;
                if (!check)
                    break;
            }
            Assert.True(check);
            check = true;
            for (int i = 0; i < 100; i++)
            {
                check = csvs2[i].Hotel == csvs2Comparer[i].Hotel;
                if (!check)
                    break;
            }
            Assert.True(check);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(memoryStream))
                {
                    sw.Write(firstCsv);
                    sw.Flush();
                    List<CsvModel> csvsComparer2 = (await memoryStream.FromCsvAsync<CsvModel>().NoContext()).ToList();
                    for (int i = 0; i < 100; i++)
                        Assert.False(csvs[i].Hotel != csvsComparer2[i].Hotel);
                }
            }
            List<CsvModel2> csvs2Comparer2 = secondCsv.Split('\n').FromCsv<CsvModel2>('|').ToList();
            check = true;
            for (int i = 0; i < 100; i++)
            {
                check = csvs2[i].Hotel == csvs2Comparer2[i].Hotel;
                if (!check)
                    break;
            }
            Assert.True(check);
        }
        private class CsvModel
        {
            public string Name { get; set; }
            public string Babel { get; set; }
            [CsvProperty("Corto")]
            public int Hotel { get; set; }
            public double Value { get; set; }
            [CsvIgnore]
            public double Nothing { get; set; }
        }
        private class CsvModel2
        {
            public string Name { get; set; }
            public string Babel { get; set; }
            [CsvProperty("Corto")]
            public int Hotel { get; set; }
            public double Value { get; set; }
            [CsvIgnore]
            public double Nothing { get; set; }
        }
    }
}
