using Microsoft.AspNetCore.Identity;

namespace RMS.Web.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Chef));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Customer));
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Driver));
            }
        } 
    }
}