using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Heracles.Lib
{
    public class HydraClient
    {

        private HttpClient httpClient;
        private string? apiToken;
        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Create a new Hydra client using the API endpoint defined by "uri".
        /// </summary>
        /// <param name="uri"></param>
        public HydraClient(string uri)
        {
            apiToken = Environment.GetEnvironmentVariable("HYDRA_TOKEN");
            ArgumentNullException.ThrowIfNull(apiToken);

            httpClient = new();
            httpClient.BaseAddress = new Uri(uri);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {apiToken}");
        }

        /// <summary>
        /// Queries Hydra for records matching a search pattern.
        /// Throws an exception if an unexpected response is recieved.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="limit"></param>
        /// <returns>A list of matching records.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<List<Record>> Get(string query = "", int limit = 500000)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"-> GET records?q={query}&limit={limit}");
            var response = await httpClient.GetAsync($"records?q={query}&limit={limit}");
            Console.WriteLine($"<- {response.StatusCode}");
            Console.ResetColor();

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException("Unexpected response from server");
            }

            var content = await response.Content.ReadAsStringAsync();
            var records = JsonSerializer.Deserialize<List<Record>>(content);
            ArgumentNullException.ThrowIfNull(records);

            Console.WriteLine(JsonSerializer.Serialize(records, options));
            return records;
        }

        /// <summary>
        /// Deletes records matching a search pattern from Hydra.
        /// Throws an exception if an unexpected response is recieved.
        /// By default, will only delete the first matching record.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="limit"></param>
        /// <returns>A list of deleted records.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<List<Record>> Delete(string query, int limit = 1)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"-> DELETE records?q=in_hostname%3A{query}&limit={limit}");
            var response = await httpClient.DeleteAsync($"records?q=in_hostname%3A{query}&limit={limit}");
            Console.WriteLine($"<- {response.StatusCode}");
            Console.ResetColor();

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException("Unexpected response from server");
            }

            var content = await response.Content.ReadAsStringAsync();
            var records = JsonSerializer.Deserialize<List<Record>>(content);
            ArgumentNullException.ThrowIfNull(records);

            Console.WriteLine(JsonSerializer.Serialize(records, options));
            return records;
        }

        /// <summary>
        /// Adds a new record to Hydra.
        /// Throws an exception if an unexpected response is recieved.
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>A copy of the newly added record.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<Record> Post(Record newRecord)
        {
            ArgumentNullException.ThrowIfNull(newRecord);
            var json = JsonSerializer.Serialize(newRecord, options);
            using StringContent jsonContent = new(json, Encoding.UTF8, "application/json");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"-> POST {json}");
            var response = await httpClient.PostAsync("records", jsonContent);
            Console.WriteLine($"<- {response.StatusCode}");
            Console.ResetColor();

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                throw new HttpRequestException("Unexpected response from server");
            }

            var content = await response.Content.ReadAsStringAsync();
            var record = JsonSerializer.Deserialize<Record>(content);
            ArgumentNullException.ThrowIfNull(record);

            Console.WriteLine(JsonSerializer.Serialize(record, options));
            return record;
        }
    }
}
