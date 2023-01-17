using CsvHelper.Configuration.Attributes;

namespace AndrewHosackServiceChannel.Data.Models
{
    public class CovidCsvModel
    {

        [Index(0)] 
        public int UID { get; set; }
        [Index(1)] 
        public string iso2 { get; set; }
        [Index(2)] 
        public string iso3 { get; set; }
        [Index(3)]
        public int? code3 { get; set; }
        [Index(4)]
        public double? FIPS { get; set; }
        [Index(5)]
        public string Admin2 { get; set; }
        [Index(6)]
        public string Province_State { get; set; }
        [Index(7)]
        public string Country_Region { get; set; }
        [Index(8)]
        public double? Lat { get; set; }
        [Index(9)]
        public double? Long_ { get; set; }
        [Index(10)]
        public string Combined_Key { get; set; }
        [Index(11)] 
        public Dictionary<string, int> CovidCases { get; set; }

    }
}
