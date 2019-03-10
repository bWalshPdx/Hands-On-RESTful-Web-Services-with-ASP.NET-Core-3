using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shouldly;
using VinylStore.Catalog.Domain.Commands.Users;
using VinylStore.Catalog.Domain.Responses.Users;
using VinylStore.Catalog.Fixtures;
using Xunit;

namespace VinylStore.Catalog.API.Tests.Controllers
{
    public class UsersControllerTests : IClassFixture<InMemoryApplicationFactory<Startup>>
    {
        private readonly InMemoryApplicationFactory<Startup> _factory;

        public UsersControllerTests(InMemoryApplicationFactory<Startup> factory)

        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/user/auth")]
        public async Task sign_in_should_retrieve_a_token(string url)
        {
            var client = _factory.CreateClient();

            var command = new SignInUserCommand { Email = "samuele.resca@example.com", Password = "P@$$w0rd" };
            var httpContent =
                new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, httpContent);
            string responseContent = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            responseContent.ShouldNotBeEmpty();
        }

        [Theory]
        [InlineData("/api/user/auth")]
        public async Task sign_in_should_retrieve_bad_request_with_invalid_password(string url)
        {
            var client = _factory.CreateClient();

            var command = new SignInUserCommand { Email = "samuele.resca@example.com", Password = "NotValidPWD" };
            var httpContent =
                new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, httpContent);
            string responseContent = await response.Content.ReadAsStringAsync();

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            responseContent.ShouldNotBeEmpty();
        }

        [Theory]
        [InlineData("/api/user")]
        public async Task get_with_authorized_user_should_retrieve_the_right_user(string url)
        {
            var client = _factory.CreateClient();


            var command = new SignInUserCommand { Email = "samuele.resca@example.com", Password = "P@$$w0rd" };
            var httpContent =
                new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url + "/auth", httpContent);
            string responseContent = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();

            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

            var restrictedResponse = await client.GetAsync(url);

            restrictedResponse.EnsureSuccessStatusCode();
            restrictedResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("/api/user")]
        public async Task post_should_create_a_new_user(string url)
        {
            var client = _factory.CreateClient();

            var command = new SignUpUserCommand
            { Email = "samuele.resca@example.com", Password = "P@$$w0rd", Name = "Samuele" };
            var httpContent =
                new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, httpContent);

            response.EnsureSuccessStatusCode();

            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Headers.Location.ToString().ShouldBe("http://localhost/api/user");
        }
    }
}