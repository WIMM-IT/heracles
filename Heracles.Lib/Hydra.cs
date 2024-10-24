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
            string? apiCredentials = Environment.GetEnvironmentVariable("HYDRA_TOKEN");
            ArgumentNullException.ThrowIfNull(apiCredentials);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(apiCredentials);
            string apiToken = Convert.ToBase64String(plainTextBytes);
            httpClient.BaseAddress = new Uri(uri);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {apiToken}");
        }

        /// <summary>
        /// Parses the Hydra API reponse, checking that it does not indicate an error.
        /// </summary>
        /// <param name="response"></param>
        /// <returns>The content of the response.</returns>
        /// <exception cref="HttpRequestException"></exception>
        private static async Task<string> ParseResponse(HttpResponseMessage response)
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
        /// Parses the returned Hydra API content into an instance of a type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JsonException"></exception>
        private static T ParseContent<T>(string content)
        {
            T? results;
            try
            {
                results = JsonSerializer.Deserialize<T>(content);
                ArgumentNullException.ThrowIfNull(results);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"CRIT: Could not parse server response into {typeof(T)}");
                Console.ResetColor();
                Console.WriteLine(content);
                throw;
            }
            return results;
        }

        /// <summary>
        /// Searches Hydra for records where the hostname contains a given substring.
        /// By default, returns unlimited matching records (up to the 500000 limit imposed by the API).
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="limit"></param>
        /// <returns>A list of matching records.</returns>
        public async Task<List<Record>> Search(string substring = "", int limit = 500000)
        {
            HttpResponseMessage response = await httpClient.GetAsync($"records?q=in_hostname%3A{substring}&limit={limit}");
            string content = await ParseResponse(response);
            return ParseContent<List<Record>>(content);
        }

        /// <summary>
        /// Deletes a record from Hydra based on its GUID.
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>A copy of the deleted record.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Record> Delete(Record theRecord)
        {
            ArgumentNullException.ThrowIfNull(theRecord.Id);
            HttpResponseMessage response = await httpClient.DeleteAsync($"records/{theRecord.Id}");
            string content = await ParseResponse(response);
            return ParseContent<Record>(content);
        }

        /// <summary>
        /// Adds a new record to Hydra.
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="theRecord"></param>
        /// <returns>A copy of the newly added record.</returns>
        public async Task<Record> Add(Record theRecord)
        {
            var json = JsonSerializer.Serialize(theRecord, options);
            using StringContent jsonContent = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync("records", jsonContent);
            string content = await ParseResponse(response);
            return ParseContent<Record>(content);
        }

        /// <summary>
        /// Updates a record in Hydra.
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="theRecord"></param>
        /// <returns>A copy of the newly updated record.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Record> Update(Record theRecord)
        {
            ArgumentNullException.ThrowIfNull(theRecord.Id);
            var json = JsonSerializer.Serialize(theRecord, options);
            using StringContent jsonContent = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PutAsync($"records/{theRecord.Id}", jsonContent);
            string content = await ParseResponse(response);
            return ParseContent<Record>(content);
        }

        /// <summary>
        /// Gets a record in Hydra.
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="theRecord"></param>
        /// <returns>A copy of the record.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Record> Get(Record theRecord)
        {
            ArgumentNullException.ThrowIfNull(theRecord.Id);
            HttpResponseMessage response = await httpClient.GetAsync($"records/{theRecord.Id}");
            string content = await ParseResponse(response);
            return ParseContent<Record>(content);
        }
    }
}
