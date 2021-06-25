using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Studio808.Api;
using Studio808.Api.Requests;
using Studio808.BusinessLogic.Components.User.Dtos;
using Xunit;

namespace Studio808.Tests
{
    public class Tests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public Tests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_User_WhenEmptyUsername_BadRequest()
        {
            HttpClient client = _factory.CreateClient();

            // POST two different users.
            string username = string.Empty;
            var createUserRequest = new CreateUserRequest(username);

            await PostAsync(client, "user", createUserRequest, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Users()
        {
            HttpClient client = _factory.CreateClient();

            // POST two different users.
            var createUser1Request = new CreateUserRequest("Alice");
            var postUser1Response = await PostAsync(client, "user", createUser1Request, HttpStatusCode.Created);
            var createUser2Request = new CreateUserRequest("Bob");
            var postUser2Response = await PostAsync(client, "user", createUser2Request, HttpStatusCode.Created);

            // Check that the created users match their intended names.
            Assert.Equal(createUser1Request.name, postUser1Response.name);
            Assert.Equal(createUser2Request.name, postUser2Response.name);
        }

        /// <summary>
        /// Post an object and check the response headers.
        /// </summary>
        private static async Task<UserDto> PostAsync(
            HttpClient client, string url, CreateUserRequest content, HttpStatusCode expectedCode)
        {
            string jsonAsString = JsonSerializer.Serialize(content);
            var stringContent = new StringContent(jsonAsString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, stringContent);
            string responseText = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<UserDto>(responseText);

            Assert.Equal(expectedCode, response.StatusCode);
            if (expectedCode == HttpStatusCode.Created)
            {
                Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
            }
            else
            {
                Assert.Equal("application/problem+json", response.Content.Headers.ContentType.MediaType);
            }

            return responseData;
        }

        /// <summary>
        /// Put an object and check the response headers.
        /// </summary>
        private static async Task<UserDto> PutAsync(HttpClient client, string url, string content)
        {
            HttpResponseMessage response = await client.PutAsync(url, new ByteArrayContent(s_charEncoding.GetBytes(content)));
            string responseText = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<UserDto>(responseText);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            return responseData;
        }

        /// <summary>
        /// Non-standard encoding to ensure binary data is handled appropriately.
        /// </summary>
        private static readonly Encoding s_charEncoding = Encoding.BigEndianUnicode;
    }
}