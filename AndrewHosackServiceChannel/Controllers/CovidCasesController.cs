using AndrewHosackServiceChannel.Data.Services;
using AndrewHosackServiceChannel.Data.Models;
using AndrewHosackServiceChannel.Data.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web.Http;
using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]")]
public class CovidCasesController : Controller
{
    private readonly ICSVService _csvService;

    public CovidCasesController(ICSVService csvService)
    {
        _csvService = csvService;
    }

    /// <summary>
    ///  Collect important statistics for either a County or State in the United States.
    /// </summary>
    /// <param name="startDate">Start Date of your Data Extract.</param>
    /// <param name="endDate">End Date of your Data Extract.</param>
    /// <param name="location">The County or State used for the Data Extraction.</param>
    [Microsoft.AspNetCore.Mvc.HttpPost("read-covidcases-csv/{location?}")]
    public async Task<IActionResult> GetCovidCasesCSV(DateTime? startDate, DateTime? endDate, string location = "notprovided")
    {
        CovidStatistics covidStatistics = new CovidStatistics();

        string locationType = _csvService.ValidateLocationInCsv(location);

        if(locationType.Equals(Constants.County) || locationType.Equals(Constants.State))
        {
            covidStatistics = _csvService.CollectCovidStatistics(locationType, location, startDate, endDate);
        }
        //if location is not found in CSV file
        else
        {
            var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(string.Format("No location named = \" {0} \"", location)),
                ReasonPhrase = Constants.LocationNotFound
            };
            throw new HttpResponseException(resp);
        }

        return Ok(covidStatistics);
    }

    /// <summary>
    ///  Collect important daily statistics for either a County or State in the United States.
    /// </summary>
    /// <param name="startDate">Start Date of your Data Extract.</param>
    /// <param name="endDate">End Date of your Data Extract.</param>
    /// <param name="location">The County or State used for the Data Extraction.</param>
    [Microsoft.AspNetCore.Mvc.HttpPost("read-covidcases-daily-csv/{location?}")]
    public async Task<IActionResult> GetDailyCovidCasesCSV(DateTime? startDate, DateTime? endDate, string location = "notprovided")
    {
        CovidDailyStatistics covidDailyStatistics = new CovidDailyStatistics();

        string locationType = _csvService.ValidateLocationInCsv(location);

        if (locationType.Equals(Constants.County) || locationType.Equals(Constants.State))
        {
            covidDailyStatistics = _csvService.CollectCovidTotalAndNewCases(locationType, location, startDate, endDate);
        }
        //if location is not found in CSV file
        else
        {
            var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(string.Format("No location named = \" {0} \"", location)),
                ReasonPhrase = Constants.LocationNotFound
            };
            throw new HttpResponseException(resp);
        }

        return Ok(covidDailyStatistics);
    }

    /// <summary>
    ///  Collect growth rate statistics for either a County or State in the United States.
    /// </summary>
    /// <param name="startDate">Start Date of your Data Extract.</param>
    /// <param name="endDate">End Date of your Data Extract.</param>
    /// <param name="location">The County or State used for the Data Extraction.</param>
    [Microsoft.AspNetCore.Mvc.HttpPost("read-covidcases-growth-csv/{location?}")]
    public async Task<IActionResult> GetCovidCasesGrowthCSV(DateTime? startDate, DateTime? endDate, string location = "notprovided")
    {
        CovidDailyGrowthStatistics covidGrowthRate = new CovidDailyGrowthStatistics();

        string locationType = _csvService.ValidateLocationInCsv(location);

        if (locationType.Equals(Constants.County) || locationType.Equals(Constants.State))
        {
            covidGrowthRate.GrowthRate = _csvService.CollectCovidGrowthRate(locationType, location, startDate, endDate);
        }
        //if location is not found in CSV file
        else
        {
            var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(string.Format("No location named = \" {0} \"", location)),
                ReasonPhrase = Constants.LocationNotFound
            };
            throw new HttpResponseException(resp);
        }

        //Json option to allow Nan to be displayed as result
        JsonSerializerOptions options = new()
        {
            NumberHandling =
                            JsonNumberHandling.AllowReadingFromString |
                            JsonNumberHandling.WriteAsString,
            WriteIndented = true
        };
        
        string jsonString = JsonSerializer.Serialize(covidGrowthRate.GrowthRate.ToString("P"), options);

        

        return Ok(jsonString);
    }



}