using Csutils;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Heracles.Lib
{
    public class HydraClient
    {

        private HttpClient httpClient;
        private string? apiToken;

        public HydraClient(string uri)
        {
            apiToken = Environment.GetEnvironmentVariable("HYDRA_TOKEN");
            ArgumentNullException.ThrowIfNull(apiToken);

            httpClient = new();
            httpClient.BaseAddress = new Uri(uri);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {apiToken}");
        }

        private void PrintRecords(List<Record> records)
        {
            foreach (var obj in records)
            {
                Console.WriteLine(obj.ToPrettyString());
            }
        }
        public async Task<List<Record>?> Get(int limit = 500000, string q = "")
        {
            Console.WriteLine($"GET records?limit={limit}&q={q}");
            List<Record>? records = await httpClient.GetFromJsonAsync<List<Record>>($"records?limit={limit}&q={q}");
            ArgumentNullException.ThrowIfNull(records);
            PrintRecords(records);
            return records;
        }

        public async Task Delete(string q)
        {
            Console.WriteLine($"DELETE records?q=in_hostname%3A{q}&limit=1");
            var response = await httpClient.DeleteAsync($"records?q=in_hostname%3A{q}&limit=1");
            List<Record>? records = JsonSerializer.Deserialize<List<Record>>(await response.Content.ReadAsStringAsync());
            ArgumentNullException.ThrowIfNull(records);
            PrintRecords(records);
        }

        public async Task Post(string host, string token)
        {
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    hostname = host,
                    type = "TXT",
                    content = token,
                    comment = "WinAcme"
                }),
                Encoding.UTF8,
                "application/json");

            Console.WriteLine($"POST {host} {token}");
            var response = await httpClient.PostAsync("records", jsonContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{jsonResponse}\n");
        }
    }
}
