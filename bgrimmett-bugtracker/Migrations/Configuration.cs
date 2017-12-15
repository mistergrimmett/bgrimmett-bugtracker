namespace bgrimmett_bugtracker.Migrations
{
    using bgrimmett_bugtracker.Models;
    using bgrimmett_bugtracker.Models.CodeFirst;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<bgrimmett_bugtracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(bgrimmett_bugtracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            //Admin
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            //Project Manager
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            //Developer
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            //Submitter
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }




            var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));
            //Admin
            if (!context.Users.Any(u => u.Email == "mistergrimmett1127@gmail.com"))
            {
                userManager.Create(new ApplicationUser         //Creating new user for the application using required fields
                {
                    UserName = "mistergrimmett1127@gmail.com",
                    DisplayName = "Brandon Grimmett",
                    Email = "mistergrimmett1127@gmail.com",
                    FirstName = "Brandon",
                    LastName = "Grimmett",
                }, "Powerman@1");
            }

            //Project Manager
            if (!context.Users.Any(u => u.Email == "mgrimmett@gmail.com"))
            {
                userManager.Create(new ApplicationUser         //Creating new user for the application using required fields
                {
                    UserName = "mgrimmett@gmail.com",
                    DisplayName = "Monty Grimmett",
                    Email = "mgrimmett@gmail.com",
                    FirstName = "Monty",
                    LastName = "Grimmett",
                }, "MontyDale@1");
            }

            //Developer
            if (!context.Users.Any(u => u.Email == "cgrimmett407@gmail.com"))
            {
                userManager.Create(new ApplicationUser         //Creating new user for the application using required fields
                {
                    UserName = "cgrimmett407@gmail.com",
                    DisplayName = "Christian Grimmett",
                    Email = "cgrimmett407@gmail.com",
                    FirstName = "Christian",
                    LastName = "Grimmett",
                }, "Christian@1");
            }

            //Submitter
            if (!context.Users.Any(u => u.Email == "kgrimmett@gmail.com"))
            {
                userManager.Create(new ApplicationUser         //Creating new user for the application using required fields
                {
                    UserName = "kgrimmett@gmail.com",
                    DisplayName = "Kaleigh Grimmett",
                    Email = "kgrimmett@gmail.com",
                    FirstName = "Kaleigh",
                    LastName = "Grimmett",
                }, "Shithead@1");
            }







            //Admin
            var adminId = userManager.FindByEmail("mistergrimmett1127@gmail.com").Id;
            userManager.AddToRole(adminId, "Admin");

            //Project Manager
            var projectManagerId = userManager.FindByEmail("mgrimmett@gmail.com").Id;
            userManager.AddToRole(projectManagerId, "Project Manager");

            //Developer
            var developerId = userManager.FindByEmail("cgrimmett407@gmail.com").Id;
            userManager.AddToRole(developerId, "Developer");

            //Submitter
            var submitterId = userManager.FindByEmail("kgrimmett@gmail.com").Id;
            userManager.AddToRole(submitterId, "Submitter");




            // hardcoding or seeding the look up tables Priority/Status/Type
            //Priority
            if (!context.TicketPriorities.Any(p => p.Name == "Low"))
            {
                var priority = new TicketPriority();
                priority.Name = "Low";
                context.TicketPriorities.Add(priority);
            }

            if (!context.TicketPriorities.Any(p => p.Name == "Medium"))
            {
                var priority = new TicketPriority();
                priority.Name = "Medium";
                context.TicketPriorities.Add(priority);
            }

            if (!context.TicketPriorities.Any(p => p.Name == "High"))
            {
                var priority = new TicketPriority();
                priority.Name = "High";
                context.TicketPriorities.Add(priority);
            }

            if (!context.TicketPriorities.Any(p => p.Name == "Urgent"))
            {
                var priority = new TicketPriority();
                priority.Name = "Urgent";
                context.TicketPriorities.Add(priority);
            }


            //Status
            if (!context.TicketStatuses.Any(p => p.Name == "Unassigned"))
            {
                var status = new TicketStatus();
                status.Name = "Unassigned";
                context.TicketStatuses.Add(status);
            }

            if (!context.TicketStatuses.Any(p => p.Name == "Assigned"))
            {
                var status = new TicketStatus();
                status.Name = "Assigned";
                context.TicketStatuses.Add(status);
            }

            if (!context.TicketStatuses.Any(p => p.Name == "In Progress"))
            {
                var status = new TicketStatus();
                status.Name = "In Progress";
                context.TicketStatuses.Add(status);
            }

            if (!context.TicketStatuses.Any(p => p.Name == "Complete"))
            {
                var status = new TicketStatus();
                status.Name = "Complete";
                context.TicketStatuses.Add(status);
            }


            //Type
            if (!context.TicketTypes.Any(p => p.Name == "Hardware"))
            {
                var type = new TicketType();
                type.Name = "Hardware";
                context.TicketTypes.Add(type);
            }

            if (!context.TicketTypes.Any(p => p.Name == "Software"))
            {
                var type = new TicketType();
                type.Name = "Software";
                context.TicketTypes.Add(type);
            }




            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
