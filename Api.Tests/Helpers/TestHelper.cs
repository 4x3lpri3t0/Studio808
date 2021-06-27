using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Helpers
{
    public class TestHelper
    {
        private const string MediaType = "application/json";

        /// <summary>
        /// Post an object and check the response headers.
        /// </summary>
        internal static async Task<T> PostAsync<T>(
            HttpClient client, string url, object content, HttpStatusCode expectedCode)
        {
            string jsonAsString = JsonSerializer.Serialize(content);
            var stringContent = new StringContent(jsonAsString, Encoding.UTF8, MediaType);
            var response = await client.PostAsync(url, stringContent);

            Assert.Equal(expectedCode, response.StatusCode);
            ValidateMediaType(response);

            string responseText = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<T>(responseText);
            return responseData;
        }

        /// <summary>
        /// Put an object and check the response headers.
        /// </summary>
        internal static async Task PutAsync(
            HttpClient client, string url, object content, HttpStatusCode expectedCode)
        {
            string jsonAsString = JsonSerializer.Serialize(content);
            var stringContent = new StringContent(jsonAsString, Encoding.UTF8, MediaType);
            var response = await client.PutAsync(url, stringContent);

            Assert.Equal(expectedCode, response.StatusCode);
            ValidateMediaType(response);
        }

        private static void ValidateMediaType(HttpResponseMessage response)
        {
            Assert.Equal(MediaType, response.Content.Headers.ContentType.MediaType);
        }
    }
}
