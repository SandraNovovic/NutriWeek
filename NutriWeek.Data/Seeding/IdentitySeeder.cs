using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using static NutriWeek.GCommon.ApplicationConstants;

namespace NutriWeek.Data.Seeding
{
    public static class IdentitySeeder
    {
        public static string[] DefaultRoles =
            new[]
            {
                UserRoleName,
                AdministratorRoleName
            };


        public static void SeedRoles(
            IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager =
                serviceProvider
                    .GetRequiredService<RoleManager<IdentityRole>>();


            foreach (string roleName
                     in DefaultRoles)
            {
                bool roleExists =
                    roleManager
                        .RoleExistsAsync(roleName)
                        .GetAwaiter()
                        .GetResult();


                if (roleExists)
                {
                    continue;
                }


                IdentityRole role =
                    new IdentityRole(roleName);


                IdentityResult result =
                    roleManager
                        .CreateAsync(role)
                        .GetAwaiter()
                        .GetResult();


                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Role {roleName} could not be created.");
                }
            }
        }
    }
}
