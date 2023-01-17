namespace AndrewHosackServiceChannel.Data.Models
{
    public class CovidDailyStatistics
    {
        public List<CovidTotalAndNewCases> CovidCases { get; set; }

    }

    public class CovidTotalAndNewCases
    {
        public string Date { get; set; }
        public int TotalCount { get; set; }
        public int DailyNewCases { get; set; }
    }
}
