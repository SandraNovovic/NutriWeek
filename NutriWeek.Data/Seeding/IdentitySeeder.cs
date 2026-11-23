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


        public static void SeedAdminUser(
            IServiceProvider serviceProvider,
            string adminEmail,
            string adminPassword)
        {
            UserManager<IdentityUser> userManager =
                serviceProvider
                    .GetRequiredService<UserManager<IdentityUser>>();


            IdentityUser? adminUser =
                userManager
                    .FindByNameAsync(adminEmail)
                    .GetAwaiter()
                    .GetResult();


            if (adminUser == null)
            {
                adminUser =
                    new IdentityUser(adminEmail)
                    {
                        Email =
                            adminEmail
                    };


                IdentityResult createResult =
                    userManager
                        .CreateAsync(
                            adminUser,
                            adminPassword)
                        .GetAwaiter()
                        .GetResult();


                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Administrator user {adminEmail} could not be created.");
                }
            }


            bool isAdministrator =
                userManager
                    .IsInRoleAsync(
                        adminUser,
                        AdministratorRoleName)
                    .GetAwaiter()
                    .GetResult();


            if (isAdministrator)
            {
                return;
            }


            IdentityResult roleResult =
                userManager
                    .AddToRoleAsync(
                        adminUser,
                        AdministratorRoleName)
                    .GetAwaiter()
                    .GetResult();


            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Administrator user {adminEmail} could not be added to role {AdministratorRoleName}.");
            }
        }
    }
}
