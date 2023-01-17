using AndrewHosackServiceChannel.Data.Models;
using AndrewHosackServiceChannel.Data.Services;

namespace AndrewHosackServiceChannel.Data.Utilities
{
    public class DataHelpers
    {
        
        public List<CovidCsvModel> SetStartDateLimit(List<CovidCsvModel> covidDataSet, DateTime? startDate)
        {
            covidDataSet.ForEach(x => x.CovidCases = x.CovidCases.Where(p => Convert.ToDateTime(p.Key) >= Convert.ToDateTime(startDate)).ToDictionary(p => p.Key, p => p.Value));
            return covidDataSet;
        }

        public List<CovidCsvModel> SetEndDateLimit(List<CovidCsvModel> covidDataSet, DateTime? endDate)
        {
            covidDataSet.ForEach(x => x.CovidCases = x.CovidCases.Where(p => Convert.ToDateTime(p.Key) <= Convert.ToDateTime(endDate)).ToDictionary(p => p.Key, p => p.Value));
            return covidDataSet;
        }

        public int AverageDailyCaseCount(List<CovidCsvModel> covidDataSet)
        {
            int totalDays;
            int totalCasesOfAllDays = 0;
            int averageCasesOfAllDays;

            //Counting total days that have been requested
            totalDays = covidDataSet.FirstOrDefault().CovidCases.Count();

            //Counting all covid cases in all provided counties
            foreach (var county in covidDataSet)
            {
                foreach (var i in county.CovidCases)
                {
                    totalCasesOfAllDays += i.Value;
                }
            }

            averageCasesOfAllDays = totalCasesOfAllDays / totalDays;

            return averageCasesOfAllDays;
        }

        public Tuple<string, int?> MinimumNumberOfCases(List<CovidCsvModel> covidDataSet)
        {
            int? minimumDailyCaseCount = null;
            string dayOfMinimumDailyCaseCount = String.Empty;

            //Looping through all covid case counts to search for the minimum case count and day
            foreach (var county in covidDataSet)
            {
                foreach (var dailyCount in county.CovidCases)
                {
                    //Setting the minimumDailyCaseCount with the first record of the dataset.
                    if (!minimumDailyCaseCount.HasValue)
                    {
                        minimumDailyCaseCount = dailyCount.Value;
                        dayOfMinimumDailyCaseCount = dailyCount.Key;
                    }
                    //If count is less than the current count then update the "new" minimum variables, or if count is the same as the current minimum but the date is "earlier"
                    if (dailyCount.Value < minimumDailyCaseCount || (dailyCount.Value.Equals(minimumDailyCaseCount) && Convert.ToDateTime(dailyCount.Key) < Convert.ToDateTime(dayOfMinimumDailyCaseCount)))
                    {
                        minimumDailyCaseCount = dailyCount.Value;
                        dayOfMinimumDailyCaseCount = dailyCount.Key;
                    }
                }
            }
            return new Tuple<string, int?>(dayOfMinimumDailyCaseCount, minimumDailyCaseCount);
            //return averageCasesOfAllDays;
        }

        public Tuple<string, int?> MaximumNumberOfCases(List<CovidCsvModel> covidDataSet)
        {
            int? maximumDailyCaseCount = null;
            string dayOfMaximumDailyCaseCount = String.Empty;

            //Looping through all covid case counts to search for the minimum case count and day
            foreach (var county in covidDataSet)
            {
                foreach (var dailyCount in county.CovidCases)
                {
                    //Setting the minimumDailyCaseCount with the first record of the dataset.
                    if (!maximumDailyCaseCount.HasValue)
                    {
                        maximumDailyCaseCount = dailyCount.Value;
                        dayOfMaximumDailyCaseCount = dailyCount.Key;
                    }
                    //If count is less than the current count then update the "new" minimum variables, or if count is the same as the current maximum but the date is "earlier"
                    if (dailyCount.Value > maximumDailyCaseCount || (dailyCount.Value.Equals(maximumDailyCaseCount) && Convert.ToDateTime(dailyCount.Key) < Convert.ToDateTime(dayOfMaximumDailyCaseCount)))
                    {
                        maximumDailyCaseCount = dailyCount.Value;
                        dayOfMaximumDailyCaseCount = dailyCount.Key;
                    }
                }
            }
            return new Tuple<string, int?>(dayOfMaximumDailyCaseCount, maximumDailyCaseCount);
        }

        public List<CovidTotalAndNewCases> CombineAllLocationsIntoOneList(List<CovidCsvModel> covidDataSet)
        {
            List<CovidTotalAndNewCases> covidDailyStatistics = new List<CovidTotalAndNewCases>();

            //Looping through all covid case counts for each included location
            foreach (var county in covidDataSet)
            {
                foreach (var dailyCount in county.CovidCases)
                {
                    var existsDate = covidDailyStatistics.Any(x => x.Date.Equals(dailyCount.Key));

                    //Date is getting added to our Daily Grand Totals for the first time
                    if (!existsDate)
                    {
                        CovidTotalAndNewCases temp = new CovidTotalAndNewCases();

                        temp.Date = dailyCount.Key;
                        temp.TotalCount = dailyCount.Value;

                        covidDailyStatistics.Add(temp);
                    }
                    //Date already exists in our Daily Grand Totals, so we will add new totals of the new location
                    else
                    {
                        //Summing up prior existing total, and adding the new daily count of the new location
                        int existingTotal = covidDailyStatistics.Where(x => x.Date.Equals(dailyCount.Key)).Select(x => x.TotalCount).FirstOrDefault();
                        existingTotal += dailyCount.Value;

                        //Updating existing List with new total
                        var obj = covidDailyStatistics.FirstOrDefault(x => x.Date.Equals(dailyCount.Key));
                        if (obj != null) obj.TotalCount = existingTotal;

                    }
                }
            }
            return covidDailyStatistics;
        }

        public CovidDailyStatistics GetTotalAndNewCases(List<CovidCsvModel> covidDataSet)
        {

            List<CovidTotalAndNewCases> covidDailyStatistics = new List<CovidTotalAndNewCases>();

            int? previousDayTotalCount = null;
            int currentDayCount;
            int currentDayNewCount;

            //If there are multiple locations in the dataset, combine them into one list
            covidDailyStatistics = CombineAllLocationsIntoOneList(covidDataSet);

            //Setting the daily change value
            foreach (var record in covidDailyStatistics)
            {

                //First record in the dataset for this location
                if (!previousDayTotalCount.HasValue)
                {
                    currentDayCount = record.TotalCount;
                    currentDayNewCount = 0;
                }
                else
                {
                    currentDayCount = record.TotalCount;
                    currentDayNewCount = currentDayCount - (int)previousDayTotalCount;
                }

                //Setting previous day total count for all other records that aren't the first record in the series
                previousDayTotalCount = record.TotalCount;

                //Updating the amount of new cases based on todays total and yesterdays count
                var obj = covidDailyStatistics.FirstOrDefault(x => x.Date.Equals(record.Date));
                if (obj != null) obj.DailyNewCases = currentDayNewCount;
            }

            CovidDailyStatistics covidDailyStatisticsList = new CovidDailyStatistics();
            covidDailyStatisticsList.CovidCases = covidDailyStatistics;
            return covidDailyStatisticsList;
        }

        public double GetGrowthRateOfDataSet(List<CovidCsvModel> covidDataSet)
        {

            List<CovidTotalAndNewCases> covidDailyStatistics = new List<CovidTotalAndNewCases>();

            //If there are multiple locations in the dataset, combine them into one list
            covidDailyStatistics = CombineAllLocationsIntoOneList(covidDataSet);

            int startingCount = covidDailyStatistics.Select(x => x.TotalCount).ToList()[0];
            int endingCount = covidDailyStatistics.Select(x => x.TotalCount).ToList()[covidDailyStatistics.Count -1];

            if (startingCount.Equals(0))
            {
                return Double.NaN;
            }

            double growthRateValue = (double)Decimal.Divide(endingCount, startingCount);


            return growthRateValue - 1;
        }
    }
}
