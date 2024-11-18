using System.Text;

namespace Heracles.Lib
{
    public class HydraClient
    {

        private readonly HttpClient httpClient = new();
        
        /// <summary>
        /// Create a new Hydra client using the API endpoint defined by "uri".
        /// Basic auth credentials should be passed in the format "user:password".
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="credentials"></param>
        /// <returns>An initialized Hydra client.</returns>
        public HydraClient(string uri, string credentials)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(credentials);
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
            throw new HttpRequestException($"Unexpected server response: {response.StatusCode}\n{content}");
        }

        /// <summary>
        /// Searches Hydra for records where any keyless entry contains a given substring.
        /// By default, returns unlimited matching records (up to the 500000 limit imposed by the API).
        /// To limit the number of records returned, set "limit" to a value from 1-500000.
        /// Throws an exception if the limit out of range, an unexpected response is recieved from the
        /// API or the deserialized JSON is null.
        /// </summary>
        /// <param name="substring"></param>
        /// <param name="limit"></param>
        /// <returns>A list of matching records.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<List<Record>> Search(string substring = "", int limit = 500000)
        {
            if (limit < 1 || limit > 500000)
            {
                throw new ArgumentOutOfRangeException("limit", "Must be between 1 and 500000");
            }
            HttpResponseMessage response = await httpClient.GetAsync($"records?q={substring}&limit={limit}");
            string content = await ParseResponse(response);
            return RecordHelpers.JsonToRecordList(content)!;
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
            return RecordHelpers.JsonToRecord(content)!;
        }

        /// <summary>
        /// Adds a new record to Hydra.
        /// Throws an exception if an unexpected response is recieved from the API or the deserialized JSON is null.
        /// </summary>
        /// <param name="theRecord"></param>
        /// <returns>A copy of the newly added record.</returns>
        public async Task<Record> Add(Record theRecord)
        {
            var json = RecordHelpers.RecordToJson(theRecord);
            using StringContent jsonContent = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync("records", jsonContent);
            string content = await ParseResponse(response);
            return RecordHelpers.JsonToRecord(content)!;
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
            var json = RecordHelpers.RecordToJson(theRecord);
            using StringContent jsonContent = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PutAsync($"records/{theRecord.Id}", jsonContent);
            string content = await ParseResponse(response);
            return RecordHelpers.JsonToRecord(content)!;
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
            return RecordHelpers.JsonToRecord(content)!;
        }
    }
}
