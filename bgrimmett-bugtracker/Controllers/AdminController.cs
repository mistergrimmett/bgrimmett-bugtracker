using bgrimmett_bugtracker.Models;
using bgrimmett_bugtracker.Models.CodeFirst.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace bgrimmett_bugtracker.Controllers
{
    [Authorize]
    //[RequireHttps]
    public class AdminController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        //[Authorize(Roles = "Admin, Project Manager, Developer, Submitter, Authenticated User")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult UserList()
        {
            List<AdminUserListModel> users = new List<AdminUserListModel>();
            UserRolesHelper helper = new UserRolesHelper(db);
            foreach (var user in db.Users.ToList())
            {
                var eachUser = new AdminUserListModel();
                eachUser.roles = new List<string>();
                eachUser.user = user;
                eachUser.roles = helper.ListUserRoles(user.Id).ToList();

                users.Add(eachUser);
            }

            ApplicationDbContext context = new ApplicationDbContext();


            var role2 = context.Roles.SingleOrDefault(u => u.Name == "Admin");
            var admins = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role2.Id)));

            var role3 = context.Roles.SingleOrDefault(u => u.Name == "Project Manager");
            var projectmanagers = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role3.Id)));

            var role4 = context.Roles.SingleOrDefault(u => u.Name == "Developer");
            var developers = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role4.Id)));

            var role5 = context.Roles.SingleOrDefault(u => u.Name == "Submitter");
            var submitters = context.Users.Where(u => u.Roles.Any(r => (r.RoleId == role5.Id)));

           
            var number2 = admins.Count();
            var number3 = projectmanagers.Count();
            var number4 = developers.Count();
            var number5 = submitters.Count();

            ViewBag.intNumber2 = number2;
            ViewBag.intNumber3 = number3;
            ViewBag.intNumber4 = number4;
            ViewBag.intNumber5 = number5;

            int[] projectTickets = { 1, 2, 3, 4, 5 };
            ViewBag.intArray = projectTickets;

            return View(users);
        }

        // GET: User List
        [Authorize(Roles = "Admin")]
        public ActionResult UserRoles(string id)
        {
            var user = db.Users.Find(id);
            UserRolesHelper helper = new UserRolesHelper(db);
            var model = new AdminUpdateRolesViewModel();

            model.Id = user.Id;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.DisplayName = user.DisplayName;
            model.SelectedRoles = helper.ListUserRoles(id).ToArray();
            model.Roles = new MultiSelectList(db.Roles, "Name", "Name", model.SelectedRoles);

            return View(model);
        }

        // POST: User List
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult UserRoles(AdminUpdateRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.Id);
                UserRolesHelper helper = new UserRolesHelper(db);

                foreach (var role in db.Roles.Select(r => r.Name).ToList())
                {
                    helper.RemoveUserFromRole(user.Id, role);
                }

                if (model.SelectedRoles != null)
                {
                    foreach (var role in model.SelectedRoles)
                    {
                        helper.AddUserToRole(user.Id, role);
                    }
                }

                return RedirectToAction("UserList", "Admin");
            }
            else
            {
                var user = db.Users.Find(model.Id);
                UserRolesHelper helper = new UserRolesHelper(db);
                var returnModel = new AdminUpdateRolesViewModel();

                returnModel.Id = user.Id;
                returnModel.FirstName = user.FirstName;
                returnModel.LastName = user.LastName;
                returnModel.DisplayName = user.DisplayName;
                returnModel.SelectedRoles = helper.ListUserRoles(user.Id).ToArray();
                returnModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", model.SelectedRoles);

                return View(returnModel);
            }
        }
    }
}