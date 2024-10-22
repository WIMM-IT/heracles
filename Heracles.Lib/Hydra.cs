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
        /// Queries Hydra for records where the hostname contains a given substring.
        /// By default, returns unlimited matching records (up to the 500000 limit imposed by the API).
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="limit"></param>
        /// <returns>A list of matching records.</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<Record>> Get(string substring = "", int limit = 500000)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"-> GET records?q=in_hostname%3A{substring}&limit={limit}");
            var response = await httpClient.GetAsync($"records?q=in_hostname%3A{substring}&limit={limit}");
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
        /// Deletes records from Hydra where the hostname contains a given substring.
        /// By default, will only delete the first matching record.
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="limit"></param>
        /// <returns>A list of deleted records.</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<Record>> Delete(string substring, int limit = 1)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"-> DELETE records?q=in_hostname%3A{substring}&limit={limit}");
            var response = await httpClient.DeleteAsync($"records?q=in_hostname%3A{substring}&limit={limit}");
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
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>A copy of the newly added record.</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
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
