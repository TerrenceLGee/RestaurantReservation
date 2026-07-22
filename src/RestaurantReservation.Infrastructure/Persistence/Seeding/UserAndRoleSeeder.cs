using Microsoft.AspNetCore.Identity;

using RestaurantReservation.Domain.Users;

namespace RestaurantReservation.Infrastructure.Persistence.Seeding;

public static class UserAndRoleSeeder
{
    public static async Task SeedUsersAndRolesAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in new[] { Roles.Admin, Roles.Manager, Roles.Customer })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var adminUser = new
        {
            FirstName = "Gordon",
            LastName = "Ramsay",
            Email = "admin@example.com",
            Password = "Pa$$w0rd",
            Roles = new[] { Roles.Admin, Roles.Manager }
        };

        var manager = new
        {
            FirstName = "Mary",
            LastName = "Wilson",
            Email = "manager@example.com",
            Password = "Pa$$w0rd",
            Roles = new[] { Roles.Manager }
        };

        var customer = new
        {
            FirstName = "Dennis",
            LastName = "Edwards",
            Email = "customer@example.com",
            Password = "Pa$$w0rd",
            Roles = new[] { Roles.Customer }
        };

        await CreateUserAsync(
            userManager,
            adminUser.FirstName,
            adminUser.LastName,
            adminUser.Email,
            adminUser.Password,
            adminUser.Roles);

        await CreateUserAsync(
            userManager,
            manager.FirstName,
            manager.LastName,
            manager.Email,
            manager.Password,
            manager.Roles);

        await CreateUserAsync(
            userManager,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Password,
            customer.Roles);
    }

    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string firstName,
        string lastName,
        string email,
        string password,
        string[] roles)
    {
        if (await userManager.FindByEmailAsync(email) is not null) return;

        var registrationDate = new DateOnly(
            DateTime.Now.Year,
            DateTime.Now.Month,
            DateTime.Now.Day);

        var user = new ApplicationUser
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = email,
            EmailConfirmed = true,
            RegistrationDate = registrationDate
        };

        await userManager.CreateAsync(user, password);
        await userManager.AddToRolesAsync(user, roles);
    }
}
