namespace AndrewHosackServiceChannel.Data.Models
{
    public class CovidStatistics
    {
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int AverageDailyCases { get; set; }
        public int? MinimumNumberCases { get; set; }
        public DateTime MinimumNumberCasesDate { get; set; }
        public int? MaximumNumberCases { get; set; }
        public DateTime MaximumNumberCasesDate { get; set; }

    }
}
