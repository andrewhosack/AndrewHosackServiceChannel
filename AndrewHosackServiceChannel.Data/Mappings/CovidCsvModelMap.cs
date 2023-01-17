using AndrewHosackServiceChannel.Data.Models;
using CsvHelper.Configuration;

public class CovidCsvModelMap : ClassMap<CovidCsvModel>
{
    public CovidCsvModelMap()
    {
        Map(m => m.UID).Index(0);
        Map(m => m.iso2).Index(1);
        Map(m => m.iso3).Index(2);
        Map(m => m.code3).Index(3);
        Map(m => m.FIPS).Index(4);
        Map(m => m.Admin2).Index(5);
        Map(m => m.Province_State).Index(6);
        Map(m => m.Country_Region).Index(7);
        Map(m => m.Lat).Index(8);
        Map(m => m.Long_).Index(9);
        Map(m => m.Combined_Key).Index(10);
        Map(m => m.CovidCases).Index(11);
    }
}