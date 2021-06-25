using Studio808.Api;
using Microsoft.AspNetCore.Mvc.Testing;
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
    }
}