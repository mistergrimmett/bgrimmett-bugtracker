using bgrimmett_bugtracker.Models;
using bgrimmett_bugtracker.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bgrimmett_bugtracker.Models.Helpers
{
    public class ProjectAssignHelper
    {
        private UserManager<ApplicationUser> userManager =
            new UserManager<ApplicationUser>(new UserStore<ApplicationUser>
                (new ApplicationDbContext()));
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var userBool = project.Users.Any(u => u.Id == userId); // access user through project
            return userBool;
        }

        public void AddUserToProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            project.Users.Add(user); // marrying project with the user
            db.SaveChanges(); // save the change to the database
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            project.Users.Remove(user);
            db.SaveChanges();
        }

        public List<Project> ListUserProjects(string userId) // List<Project> not ICollection<ApplicationUser>, virtual properties makes life a lot easier because it gives access to all the tables
        {
            var user = db.Users.Find(userId);
            return user.Projects.ToList();
        }

        public ICollection<ApplicationUser> ListUsersOnProject(int projectId)
        {
            var project = db.Projects.Find(projectId);
            return project.Users.ToList();
        }

        public ICollection<ApplicationUser> ListUsersNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }
    }
}
