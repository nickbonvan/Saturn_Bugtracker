namespace Saturn_BugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Saturn_BugTracker.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Saturn_BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Saturn_BugTracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "nicholas.bonvan@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    FirstName = "Nicholas",
                    LastName = "Bonvan",
                    DisplayName = "Nick Bonvan",
                    PhoneNumber = "(516) 987-2487",
                    UserName = "nicholas.bonvan@gmail.com",
                    Email = "nicholas.bonvan@gmail.com",
                }, "Password-1");
            }

            if (!context.Users.Any(u => u.Email == "sszpunar@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    FirstName = "Sean",
                    LastName = "Szpunar",
                    DisplayName = "Sean Szpunar (CF)",
                    PhoneNumber = "(704) 975-4882",
                    UserName = "sszpunar@coderfoundry.com",
                    Email = "sszpunar@coderfoundry.com",
                }, "Password-1");
            }

            if (!context.Users.Any(u => u.Email == "pm@pm.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    FirstName = "Project",
                    LastName = "Manager",
                    DisplayName = "ProjectManager",
                    PhoneNumber = "5555555555",
                    UserName = "pm@pm.com",
                    Email = "pm@pm.com",
                }, "Password-1");
            }

            if (!context.Users.Any(u => u.Email == "dev@dev.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    FirstName = "Developer",
                    LastName = "Developer",
                    DisplayName = "Developer",
                    PhoneNumber = "5555555555",
                    UserName = "dev@dev.com",
                    Email = "dev@dev.com",
                }, "Password-1");
            }

            if (!context.Users.Any(u => u.Email == "submitter@submitter.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    FirstName = "Submitter",
                    LastName = "Submitter",
                    DisplayName = "Submitter",
                    PhoneNumber = "5555555555",
                    UserName = "submitter@submitter.com",
                    Email = "submitter@submitter.com",
                }, "Password-1");
            }

            if (!context.Users.Any(u => u.Email == "admin@bugtracker.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    DisplayName = "Bugtracker Admin",
                    PhoneNumber = "5555555555",
                    UserName = "admin@bugtracker.com",
                    Email = "admin@bugtracker.com",
                }, "Password-1");
            }

            if (!context.Users.Any(u => u.Email == "project.manager@bugtracker.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    FirstName = "Project",
                    LastName = "Manager",
                    DisplayName = "Bugtracker Project Manager",
                    PhoneNumber = "5555555555",
                    UserName = "project.manager@bugtracker.com",
                    Email = "project.manager@bugtracker.com",
                }, "Password-1");
            }

            if (!context.Users.Any(u => u.Email == "developer@bugtracker.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    FirstName = "Developer",
                    LastName = "Developer",
                    DisplayName = "Bugtracker Developer",
                    PhoneNumber = "5555555555",
                    UserName = "developer@bugtracker.com",
                    Email = "developer@bugtracker.com",
                }, "Password-1");
            }

            if (!context.Users.Any(u => u.Email == "submitter@bugtracker.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    FirstName = "Submitter",
                    LastName = "Submitter",
                    DisplayName = "Bugtracker Submitter",
                    PhoneNumber = "5555555555",
                    UserName = "submitter@bugtracker.com",
                    Email = "submitter@bugtracker.com",
                }, "Password-1");
            }

            var userId_Nick = userManager.FindByEmail("nicholas.bonvan@gmail.com").Id;
            userManager.AddToRole(userId_Nick, "Admin");
            userManager.AddToRole(userId_Nick, "Developer");

            var userId_Sean = userManager.FindByEmail("sszpunar@coderfoundry.com").Id;
            userManager.AddToRole(userId_Sean, "Admin");

            var userId_PM = userManager.FindByEmail("pm@pm.com").Id;
            userManager.AddToRole(userId_PM, "Project Manager");

            var userId_Dev = userManager.FindByEmail("dev@dev.com").Id;
            userManager.AddToRole(userId_Dev, "Developer");

            var userId_Submitter = userManager.FindByEmail("submitter@submitter.com").Id;
            userManager.AddToRole(userId_Submitter, "Submitter");

            userManager.AddToRole(userManager.FindByEmail("admin@bugtracker.com").Id, "Admin");
            userManager.AddToRole(userManager.FindByEmail("project.manager@bugtracker.com").Id, "Project Manager");
            userManager.AddToRole(userManager.FindByEmail("developer@bugtracker.com").Id, "Developer");
            userManager.AddToRole(userManager.FindByEmail("submitter@bugtracker.com").Id, "Submitter");

            context.Priorities.AddOrUpdate(p => p.Id,
                new Priority() { Id = 1, Name = "New" },
                new Priority() { Id = 2, Name = "Lowest" },
                new Priority() { Id = 3, Name = "Low" },
                new Priority() { Id = 4, Name = "Medium" },
                new Priority() { Id = 5, Name = "High" },
                new Priority() { Id = 6, Name = "Highest" },
                new Priority() { Id = 7, Name = "Emergency" });

            context.Types.AddOrUpdate(p => p.Id,
                new Models.Type() { Id = 1, Name = "Software Issue" },
                new Models.Type() { Id = 2, Name = "Hardware Issue" },
                new Models.Type() { Id = 3, Name = "Network Issue" },
                new Models.Type() { Id = 4, Name = "Text Issue" },
                new Models.Type() { Id = 5, Name = "Feature Request" });

            context.Statuses.AddOrUpdate(p => p.Id,
                new Status() { Id = 1, Name = "New" },
                new Status() { Id = 2, Name = "Open" },
                new Status() { Id = 3, Name = "Awaiting Response" },
                new Status() { Id = 4, Name = "In Progress" },
                new Status() { Id = 5, Name = "Closed" },
                new Status() { Id = 6, Name = "Resolved" },
                new Status() { Id = 7, Name = "Deleted" });
        }
    }
}
