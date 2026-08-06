using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using RestaurantReservation.Application.Features.Auth.Command.Login;

namespace RestaurantReservation.IntegrationTests.Auth;

public class LogoutTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Logout_Returns_StatusCode_200OK_WhenSuccessful()
    {
        var refreshToken = await GetRefreshToken();

        var logoutResponse = await Client.PostAsJsonAsync("/api/auth/logout",
            new { RefreshToken = refreshToken },
            TestContext.Current.CancellationToken);

        var logoutMessage = await logoutResponse.Content.ReadFromJsonAsync<string>(
            TestContext.Current.CancellationToken);

        logoutResponse.IsSuccessStatusCode.Should().BeTrue();
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        logoutMessage.Should().NotBeNull();
        logoutMessage.Should().Be("Logout successful");
    }

    [Fact]
    public async Task Logout_Returns_StatusCode_403Forbidden_WhenTryingToRevoke_InvalidRefreshToken()
    {
        var loginToken = await GetRefreshToken();

        var refreshToken = $"{loginToken}19238hksdnsdadcewzxse12034";

        var logoutResponse = await Client.PostAsJsonAsync("/api/auth/logout", new { RefreshToken = refreshToken },
            TestContext.Current.CancellationToken);

        logoutResponse.IsSuccessStatusCode.Should().BeFalse();
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<string> GetRefreshToken()
    {
        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login",
            new { Email = "customer@example.com", Password = "Pa$$w0rd" },
            TestContext.Current.CancellationToken);

        loginResponse.IsSuccessStatusCode.Should().BeTrue();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(
            TestContext.Current.CancellationToken);

        loginResult.Should().NotBeNull();

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

        return loginResult.RefreshToken;
    }
}