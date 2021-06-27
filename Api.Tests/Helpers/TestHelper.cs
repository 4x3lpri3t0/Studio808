using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Api.Requests;
using BusinessLogic.Components.UserComponent.Dtos;
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

        /// <summary>
        /// Get an object that is expected to exist and check the response headers.
        /// </summary>
        internal static async Task<HttpResponseMessage> GetAsync(
            HttpClient client, string url, HttpStatusCode expectedCode)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            Assert.Equal(expectedCode, response.StatusCode);
            ValidateMediaType(response);

            return response;
        }

        /// <summary>
        /// Get an object and deserialize to a custom type.
        /// </summary>
        internal static async Task<T> GetAsync<T>(
            HttpClient client, string url, HttpStatusCode expectedCode)
        {
            var response = await GetAsync(client, url, expectedCode);
            string responseText = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<T>(responseText);
            return responseData;
        }

        internal static async Task<UserDto> CreateUser(HttpClient client)
        {
            string userUrl = "user";
            var userRequest = new CreateUserRequest("Alice");

            return await PostAsync<UserDto>(client, userUrl, userRequest, HttpStatusCode.Created);
        }

        private static void ValidateMediaType(HttpResponseMessage response)
        {
            Assert.Equal(MediaType, response.Content.Headers.ContentType.MediaType);
        }
    }
}
