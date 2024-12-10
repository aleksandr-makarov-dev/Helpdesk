using Helpdesk.API.Configuration;
using Helpdesk.API.Modules.Tickets;
using Helpdesk.API.Modules.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Helpdesk.API.Domain
{
    public static class Seeder
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var applicationOptions = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationOptions>>().Value;

                if (! await roleManager.Roles.AnyAsync())
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(Role.Admin));
                    await roleManager.CreateAsync(new IdentityRole<Guid>(Role.User));
                }

                if (!await userManager.Users.AnyAsync())
                {
                    var admin = new User
                    {
                        Email = applicationOptions.DefaultAdminEmail,
                        UserName = applicationOptions.DefaultAdminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin,applicationOptions.DefaultAdminPassword);

                    if (!result.Succeeded)
                    {
                        throw new Exception("Failed to create default admin user");
                    }

                    await userManager.AddToRoleAsync(admin, Role.Admin);
                }
            }
        }
    }
}
