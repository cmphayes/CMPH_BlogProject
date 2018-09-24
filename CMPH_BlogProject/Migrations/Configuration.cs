namespace CMPH_BlogProject.Migrations
{
    using CMPH_BlogProject.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CMPH_BlogProject.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(CMPH_BlogProject.Models.ApplicationDbContext context)
        {
            //create roles
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Mod"))
            {
                roleManager.Create(new IdentityRole { Name = "Mod" });
            }
            //create users
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            //create user to assign to admin role
            if (!context.Users.Any(u => u.Email == "1234@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "1234@Mailinator.com",
                    Email = "1234@Mailinator.com",
                    FirstName = "Conall",
                    LastName = "Hayes",
                    DisplayName = "ConallHayes"
                }, "Abcd1234!");
            }
            if (!context.Users.Any(u => u.Email == "cmphayes@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "cmphayes@gmail.com",
                    Email = "cmphayes@gmail.com",
                    FirstName = "Conall",
                    LastName = "Hayes",
                    DisplayName = "ConallHayes"
                }, "Abcd1234!");
            }
            var adminId = userManager.FindByEmail("cmphayes@gmail.com").Id;
            userManager.AddToRole(adminId, "Admin");
            //create user to assign to mod role
            if (!context.Users.Any(u => u.Email == "Moderator@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "Moderator@Mailinator.com",
                    Email = "Moderator@Mailinator.com",
                    FirstName = "Mod",
                    LastName = "Erator",
                    DisplayName = "Moderator"
                }, "Abcd1234!");
            }
            //assign users to roles
            var modId = userManager.FindByEmail("Moderator@Mailinator.com").Id;
            userManager.AddToRole(modId, "Mod");
        }




    }
}
