using AndrewHosackServiceChannel.Data.Models;

namespace AndrewHosackServiceChannel.Data.Services
{
    public interface ICSVService
    {
        public List<CovidCsvModel> ReadCsvForCounty(string county);
        public List<CovidCsvModel> ReadCsvForState(string state);
        //public bool ValidateCountyNameInCsv(string county);
        //public bool ValidateStateNameInCsv(string state);
        public string ValidateLocationInCsv(string location);
        public CovidStatistics CollectCovidStatistics(string locationType, string location, DateTime? startDate, DateTime? endDate);
        public CovidDailyStatistics CollectCovidTotalAndNewCases(string locationType, string location, DateTime? startDate, DateTime? endDate);
        public double CollectCovidGrowthRate(string locationType, string location, DateTime? startDate, DateTime? endDate);
    }
}
