
using System.Text.Json;

namespace PredicaoQuantidadeJogadoresSeriesTemporais.Data;

public class DataManager
{
    private static readonly HttpClient httpClient = new HttpClient();

    private List<int> data = new List<int>();

    public bool GettingData { get; private set; }

    public List<int> Data => data;

    public async Task<List<int>> DataExtraction(int months, int appId)
    {
        if (months <= 0)
            throw new ArgumentException("A quantidade de meses deve ser maior que zero.");

        GettingData = true;

        try
        {
            data = await GetLastMonthsPlayerPeakAsync(months, appId);
            return data;
        }
        finally
        {
            GettingData = false;
        }
    }

    private async Task<List<int>> GetLastMonthsPlayerPeakAsync(
        int months,
        int appId)
    {
        string url = $"https://steamcharts.com/app/{appId}/chart-data.json";

        using HttpRequestMessage request = new HttpRequestMessage(
            HttpMethod.Get,
            url
        );

        request.Headers.Add(
            "User-Agent",
            "Mozilla/5.0"
        );

        using HttpResponseMessage response =
            await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        List<List<long>>? records =
            JsonSerializer.Deserialize<List<List<long>>>(json);

        if (records == null)
            throw new Exception("Não foi possível interpretar os dados do SteamCharts.");

        // Início do mês atual em UTC.
        DateTime currentMonth = new DateTime(
            DateTime.UtcNow.Year,
            DateTime.UtcNow.Month,
            1,
            0, 0, 0,
            DateTimeKind.Utc
        );

        // Primeiro mês que deverá entrar na extração.
        DateTime firstMonth = currentMonth.AddMonths(-months);

        // Filtra somente os registros dos meses completos desejados.
        var monthlyPeaks = records
            .Where(record => record.Count >= 2)
            .Select(record => new
            {
                Date = DateTimeOffset
                    .FromUnixTimeMilliseconds(record[0])
                    .UtcDateTime,

                Players = record[1]
            })
            .Where(record =>
                record.Date >= firstMonth &&
                record.Date < currentMonth
            )
            .GroupBy(record => new DateTime(
                record.Date.Year,
                record.Date.Month,
                1
            ))
            .ToDictionary(
                group => group.Key,
                group => group.Max(record => record.Players)
            );

        List<int> result = new List<int>();

        // Percorre os meses em ordem cronológica.
        for (int i = 0; i < months; i++)
        {
            DateTime month = firstMonth.AddMonths(i);

            if (!monthlyPeaks.TryGetValue(month, out long peak))
            {
                throw new Exception(
                    $"Não existem registros para {month:MM/yyyy}."
                );
            }

            result.Add(checked((int)peak));
        }

        return result;
    }
}
