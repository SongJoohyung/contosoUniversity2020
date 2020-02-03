using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace contosoUniversity2020.Data
{
    //mwilliams: Part 11: IDENTITY FRAMEWORK
    //1. Seeding roles and admin user
    public class IdentityDbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider,
                                                            string adminUserPW)
        {
            //1. Initialize custom roles
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //2. These will be the roles
            string[] roleNames = { "Admin", "Student", "Instructor" };

            //3. Prepare result variable 
            IdentityResult roleResult;

            //4. Loop the roleNames array - check if role already exists, and create new role if
            //   neccessary
            foreach (var roleName in roleNames)
            {
                //check if exists
                var roleExist = await RoleManager.RoleExistsAsync(roleName);

                if(!roleExist)
                {
                    //role does not exist - create it
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            //END CREATE ROLES


            //CREATE ADMIN USER
            //1. Initialize custom user(s) - in this case, just the admin user
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            //2. Check if the admin user already exists
            IdentityUser adminUser = await UserManager.FindByEmailAsync("admin@contoso.com");

            if(adminUser == null)
            {
                //admin user does not already exist - create it
                adminUser = new IdentityUser()
                {
                    UserName = "admin@contoso.com",
                    Email = "admin@contoso.com"
                };
                //now actually create it
                await UserManager.CreateAsync(adminUser, adminUserPW);
                //the adminUserPw is for storing the password - it will be passed in via
                //Startup (using Dependency Injection). The actual password will be kept in
                //a file called secrats.json

                //Assign adminUser to AdminRole
                await UserManager.AddToRoleAsync(adminUser, "Admin");

                //Manually confirm the admin user's email
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(adminUser);
                var result = await UserManager.ConfirmEmailAsync(adminUser, code);

                //No Account for admin user
                await UserManager.SetLockoutEnabledAsync(adminUser, false);
            }

        }
    }
}
