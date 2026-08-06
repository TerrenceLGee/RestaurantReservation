using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

namespace RestaurantReservation.IntegrationTests.Auth;

public class RegisterUserTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task RegisterUser_Returns_200OK_Upon_Successful_User_Registration()
    {
        var registrationResponse = await Client.PostAsJsonAsync("/api/auth/register",
            new
            {
                FirstName = "Indiana",
                LastName = "Jones",
                EmailAddress = "indyJones@example.com",
                PhoneNumber = "555-987-6543",
                Password = "RaidersOfTheLostArk123$",
                ConfirmPassword = "RaidersOfTheLostArk123$"
            },
            TestContext.Current.CancellationToken);

        var successMessage = await registrationResponse.Content.ReadFromJsonAsync<string>(
            TestContext.Current.CancellationToken);

        registrationResponse.IsSuccessStatusCode.Should().BeTrue();
        registrationResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        successMessage.Should().Be("Registration successful!");
    }

    [Fact]
    public async Task
        RegisterUser_Returns_400BadRequest_WhenTryingRegistration_WherePassword_And_ConfirmationPassword_DoNotMatch()
    {
        var registrationResponse = await Client.PostAsJsonAsync("/api/auth/register",
            new
            {
                FirstName = "Han",
                LastName = "Solo",
                EmailAddress = "hSolo@example.com",
                Password = "TheEmpireStrikesBack123@",
                ConfirmPassword = "TheEmpireStrikesBack123$"
            },
            TestContext.Current.CancellationToken);

        registrationResponse.IsSuccessStatusCode.Should().BeFalse();
        registrationResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}