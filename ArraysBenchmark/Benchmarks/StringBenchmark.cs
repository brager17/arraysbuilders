using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace ConsoleApp1
{
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter, PlainExporter] [MemoryDiagnoser]
    public class StringBenchmark
    {
        private IEnumerable<string> EnumerableStrings;

        [Params(
            10000,
            20000,
            30000,
            40000,
            50000,
            60000,
            70000,
            80000,
            90000,
            100000,
            110000,
            120000,
            130000,
            140000,
            150000,
            160000,
            170000,
            180000,
            190000,
            200000,
            21000,
            220000,
            230000,
            240000,
            250000,
            260000,
            270000,
            280000,
            290000,
            300000,
            31000,
            320000,
            330000,
            340000,
            350000,
            360000,
            370000,
            380000,
            390000,
            400000,
            41000,
            420000,
            430000,
            440000,
            450000,
            460000,
            470000,
            480000,
            490000,
            500000,
            51000,
            520000,
            530000,
            540000,
            550000,
            560000,
            570000,
            580000,
            590000,
            600000,
            610000,
            620000,
            630000,
            640000,
            650000,
            660000,
            670000,
            680000,
            690000,
            700000,
            710000,
            720000,
            730000,
            740000,
            750000,
            760000,
            770000,
            780000,
            790000,
            800000,
            810000,
            820000,
            830000,
            840000,
            850000,
            860000,
            870000,
            880000,
            890000,
            900000,
            910000,
            920000,
            930000,
            940000,
            950000,
            960000,
            970000,
            980000,
            990000,
            1000000,
            1000010,
            1000020,
            1000030,
            1000040,
            1000050,
            1000060,
            1000070,
            1000080,
            1000090,
            1000100,
            1000110
        )]
        public int _count;

        public StringBenchmark()
        {
            EnumerableStrings = GetEnumerableStrings();
        }


        private IEnumerable<string> GetEnumerableStrings()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return "string";
            }
        }

        [Benchmark]
        public void ToArrayString()
        {
            var r = EnumerableStrings.ToArray();
        }


        [Benchmark]
        public void ToListString()
        {
            var r = EnumerableStrings.ToList();
        }
    }
}