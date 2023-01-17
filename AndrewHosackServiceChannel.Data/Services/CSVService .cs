using AndrewHosackServiceChannel.Data.Models;
using AndrewHosackServiceChannel.Data.Services;
using AndrewHosackServiceChannel.Data.Utilities;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

public class CSVService : ICSVService
{
    private readonly DataHelpers _dataHelpers;

    public CSVService(DataHelpers dataHelpers)
    {
        _dataHelpers = dataHelpers;
    }
    public List<CovidCsvModel> ReadCsvForCounty(string county)
    {
        List<CovidCsvModel> result;

        using (var reader = new StreamReader(Constants.CovidCsvFileName))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<CovidCsvModelMap>();

            result = csv.GetRecords<CovidCsvModel>()
                .Where(x => x.Admin2.ToLower().Equals(county.ToLower())).ToList();
        }

        return result;
    }

    public List<CovidCsvModel> ReadCsvForState(string state)
    {
        List<CovidCsvModel> result;

        using (var reader = new StreamReader(Constants.CovidCsvFileName))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<CovidCsvModelMap>();

            result = csv.GetRecords<CovidCsvModel>()
                .Where(x => x.Province_State.ToLower().Equals(state.ToLower())).ToList();
        }

        return result;
    }

    public bool ValidateCountyNameInCsv(string county)
    {
        bool exists;
        using (var reader = new StreamReader(Constants.CovidCsvFileName))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            exists = csv.GetRecords<CovidCsvModel>().Any(x => x.Admin2.ToLower().Equals(county.ToLower()));
        }
        return exists;
    }

    public bool ValidateStateNameInCsv(string state)
    {
        bool exists;
        using (var reader = new StreamReader(Constants.CovidCsvFileName))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            exists = csv.GetRecords<CovidCsvModel>().Any(x => x.Province_State.ToLower().Equals(state.ToLower()));
        }
        return exists; 
    }

    public string ValidateLocationInCsv(string location)
    {
        string locationType = String.Empty;

        //Check if location is a County
        locationType = ValidateCountyNameInCsv(location) ? Constants.County : "";

        //Check if location is a State
        if (string.IsNullOrEmpty(locationType))
        {
            locationType = ValidateStateNameInCsv(location) ? Constants.State : "";
        }

        //location is neither a County or a State
        if (string.IsNullOrEmpty(locationType))
        {
            locationType = Constants.LocationNotFound;
        }

        return locationType;
    }

    public CovidStatistics CollectCovidStatistics(string locationType, string location, DateTime? startDate, DateTime? endDate)
    {
        List<CovidCsvModel> result = new List<CovidCsvModel>();

        if (locationType.Equals(Constants.County))
        {
            result = ReadCsvForCounty(location);
        }
        if (locationType.Equals(Constants.State))
        {
            result = ReadCsvForState(location);
        }

        if (startDate.HasValue)
        {
            //Filtering on start date
            result = _dataHelpers.SetStartDateLimit(result, startDate);
        }

        if (endDate.HasValue)
        {
            //Filtering on end date
            result = _dataHelpers.SetEndDateLimit(result, endDate);
        }

        //Determine average daily case count
        int averageDailyCaseCounts = _dataHelpers.AverageDailyCaseCount(result);

        //Get Minimum Daily Case Count and Date it occurred
        var getMinimumDailyCaseCount = _dataHelpers.MinimumNumberOfCases(result);
        string minimumCaseDate = getMinimumDailyCaseCount.Item1;
        int? minimumCaseCount = getMinimumDailyCaseCount.Item2;

        //Get Maximum Daily Case Count and Date it occurred
        var getMaximumDailyCaseCount = _dataHelpers.MaximumNumberOfCases(result);
        string maximumCaseDate = getMaximumDailyCaseCount.Item1;
        int? maximumCaseCount = getMaximumDailyCaseCount.Item2;

        //Build Data Object to send back to user
        CovidStatistics covidStatistics = new CovidStatistics();

        covidStatistics.Location = location;
        
        //Latitude and Longitude required for County
        if(locationType.Equals(Constants.County))
        {
            covidStatistics.Latitude = result.Select(x => x.Lat).FirstOrDefault();
            covidStatistics.Longitude = result.Select(x => x.Long_).FirstOrDefault();
        }
        //Latitude and Longitude not required for State
        else
        {
            covidStatistics.Latitude = null;
            covidStatistics.Longitude = null;
        }

        covidStatistics.AverageDailyCases = averageDailyCaseCounts;
        covidStatistics.MinimumNumberCasesDate = Convert.ToDateTime(minimumCaseDate);
        covidStatistics.MinimumNumberCases = minimumCaseCount;
        covidStatistics.MaximumNumberCasesDate = Convert.ToDateTime(maximumCaseDate);
        covidStatistics.MaximumNumberCases = maximumCaseCount;

        return covidStatistics;

    }

    public CovidDailyStatistics CollectCovidTotalAndNewCases(string locationType, string location, DateTime? startDate, DateTime? endDate)
    {
        List<CovidCsvModel> result = new List<CovidCsvModel>();

        if (locationType.Equals(Constants.County))
        {
            result = ReadCsvForCounty(location);
        }
        if (locationType.Equals(Constants.State))
        {
            result = ReadCsvForState(location);
        }

        if (startDate.HasValue)
        {
            //Filtering on start date
            result = _dataHelpers.SetStartDateLimit(result, startDate);
        }

        if (endDate.HasValue)
        {
            //Filtering on end date
            result = _dataHelpers.SetEndDateLimit(result, endDate);
        }

        CovidDailyStatistics covidDailyStatistics = new CovidDailyStatistics();

        covidDailyStatistics = _dataHelpers.GetTotalAndNewCases(result);

        return covidDailyStatistics;

    }

    public double CollectCovidGrowthRate(string locationType, string location, DateTime? startDate, DateTime? endDate)
    {
        List<CovidCsvModel> result = new List<CovidCsvModel>();

        if (locationType.Equals(Constants.County))
        {
            result = ReadCsvForCounty(location);
        }
        if (locationType.Equals(Constants.State))
        {
            result = ReadCsvForState(location);
        }

        if (startDate.HasValue)
        {
            //Filtering on start date
            result = _dataHelpers.SetStartDateLimit(result, startDate);
        }

        if (endDate.HasValue)
        {
            //Filtering on end date
            result = _dataHelpers.SetEndDateLimit(result, endDate);
        }

        double covidGrowthRate;

        covidGrowthRate = _dataHelpers.GetGrowthRateOfDataSet(result);

        return covidGrowthRate;

    }



}