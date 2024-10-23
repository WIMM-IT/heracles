using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Heracles.Lib
{
    public class HydraClient
    {

        private readonly HttpClient httpClient = new();
        private readonly string? apiToken;
        private readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Create a new Hydra client using the API endpoint defined by "uri".
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>A new Hydra client.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public HydraClient(string uri)
        {
            apiToken = Environment.GetEnvironmentVariable("HYDRA_TOKEN");
            ArgumentNullException.ThrowIfNull(apiToken);
            httpClient.BaseAddress = new Uri(uri);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {apiToken}");
        }

        /// <summary>
        /// Validates that the Hydra API reponse does not indicate an error.
        /// </summary>
        /// <param name="response"></param>
        /// <returns>The content of the response.</returns>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<string> GetContent(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return content;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("CRIT: Unexpected response from server");
            Console.ResetColor();
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(content);
            throw new HttpRequestException();
        }

        /// <summary>
        /// Searches Hydra for records where the hostname contains a given substring.
        /// By default, returns unlimited matching records (up to the 500000 limit imposed by the API).
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="limit"></param>
        /// <returns>A list of matching records.</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<Record>> Search(string substring = "", int limit = 500000)
        {
            HttpResponseMessage response = await httpClient.GetAsync($"records?q=in_hostname%3A{substring}&limit={limit}");
            string content = await GetContent(response);
            var records = JsonSerializer.Deserialize<List<Record>>(content);
            ArgumentNullException.ThrowIfNull(records);
            
            return records;
        }

        /// <summary>
        /// Deletes a record from Hydra based on its GUID.
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>A copy of the deleted record.</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Record> Delete(Record theRecord)
        {
            ArgumentNullException.ThrowIfNull(theRecord.Id);

            HttpResponseMessage response = await httpClient.DeleteAsync($"records/{theRecord.Id}");
            string content = await GetContent(response);
            var record = JsonSerializer.Deserialize<Record>(content);
            ArgumentNullException.ThrowIfNull(record);
            
            return record;
        }

        /// <summary>
        /// Adds a new record to Hydra.
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="theRecord"></param>
        /// <returns>A copy of the newly added record.</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Record> Add(Record theRecord)
        {
            var json = JsonSerializer.Serialize(theRecord, options);
            using StringContent jsonContent = new(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync("records", jsonContent);
            string content = await GetContent(response);
            var record = JsonSerializer.Deserialize<Record>(content);
            ArgumentNullException.ThrowIfNull(record);
            
            return record;
        }

        /// <summary>
        /// Updates a record in Hydra.
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="theRecord"></param>
        /// <returns>A copy of the newly updated record.</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Record> Update(Record theRecord)
        {
            ArgumentNullException.ThrowIfNull(theRecord.Id);
            var json = JsonSerializer.Serialize(theRecord, options);
            using StringContent jsonContent = new(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PutAsync($"records/{theRecord.Id}", jsonContent);
            string content = await GetContent(response);
            var record = JsonSerializer.Deserialize<Record>(content);
            ArgumentNullException.ThrowIfNull(record);
            
            return record;
        }

        /// <summary>
        /// Gets a record in Hydra.
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="theRecord"></param>
        /// <returns>A copy of the record.</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Record> Get(Record theRecord)
        {
            ArgumentNullException.ThrowIfNull(theRecord.Id);

            HttpResponseMessage response = await httpClient.GetAsync($"records/{theRecord.Id}");
            string content = await GetContent(response);
            var record = JsonSerializer.Deserialize<Record>(content);
            ArgumentNullException.ThrowIfNull(record);
            
            return record;
        }
    }
}
