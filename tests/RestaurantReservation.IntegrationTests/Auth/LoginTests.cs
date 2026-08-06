using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Auth.Command.Login;

namespace RestaurantReservation.IntegrationTests.Auth;

public class LoginTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Login_Returns_StatusCode_200OK_WhenSuccessful()
    {
        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login",
            new { Email = "customer@example.com", Password = "Pa$$w0rd" },
            TestContext.Current.CancellationToken);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(
            TestContext.Current.CancellationToken);

        loginResponse.IsSuccessStatusCode.Should().BeTrue();
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        loginResult.Should().NotBeNull();
        loginResult.AccessToken.Should().BeOfType<string>();
        loginResult.RefreshToken.Should().BeOfType<string>();
    }

    [Fact]
    public async Task Login_Returns_StatusCode_404NotFound_WhenUnregisteredUser_TriesToLogin()
    {
        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login",
            new { Email = "unknown@example.com", Password = "Unkn0wnPa$$w0rd" },
            TestContext.Current.CancellationToken);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(
            TestContext.Current.CancellationToken);

        loginResponse.IsSuccessStatusCode.Should().BeFalse();
        loginResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        loginResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_Returns_StatusCode_401Unauthorized_WhenUser_EntersWrongPassword()
    {
        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login",
            new { Email = "customer@example.com", Password = "Passw0rd" },
            TestContext.Current.CancellationToken);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(
            TestContext.Current.CancellationToken);

        loginResponse.IsSuccessStatusCode.Should().BeFalse();
        loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        loginResult.Should().NotBeNull();
    }
}