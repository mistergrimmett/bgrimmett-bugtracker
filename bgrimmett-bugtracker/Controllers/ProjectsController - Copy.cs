﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bgrimmett_bugtracker.Models;
using bgrimmett_bugtracker.Models.CodeFirst;
using PagedList;
using Microsoft.AspNet.Identity;
using bgrimmett_bugtracker.Models.CodeFirst.Helpers;
using bgrimmett_bugtracker.Models.Helpers;

namespace bgrimmett_bugtracker.Controllers
{
    public class _ProjectsController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext(); 

        // GET: Projects
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Index()
        {
            //ProjectUserViewModels projectuservm = new ProjectUserViewModels();
            //find project based on project id
            var projects = db.Projects;
            //find user for this project id
            //var assignmentList = db.ProjectUsers.Where(a => a.ProjectId == project.Id).ToList();
            //var i = 0;
            //foreach (var assignment in assignmentList)
            //{
            //    var user = db.Users.Find(assignment.UserId);
            //    projectuservm.SelectedUsers[i] = user.DisplayName;
            //    i++;
            //}
            //projectuservm.SelectedUsers = helper.ListAssignedUsers(id).Select(u => u.Id).ToArray();
            //projectuservm.Users = new MultiSelectList(db.Users.Where(u => (u.DisplayName != "N/A" && u.DisplayName != "(Remove Assigned User)")).OrderBy(u => u.FirstName), "Id", "DisplayName", projectuservm.SelectedUsers);

            //projectuservm.AssignProject = project;


            return View(projects);
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }


        //GET: Projects/MyProjects
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        // GET: Assigned Tickets to user
        public ActionResult MyProjects(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var user = db.Users.Find(User.Identity.GetUserId());

            return View(db.Projects.Where(a => user.Id.Contains(a.AssignedToUserId)).OrderByDescending(p => p.DateCreated).ToPagedList(pageNumber, pageSize));
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Name,AuthorId,Body,IsActive,DateCreated, DateUpdated, AssignedToUserId")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", project.AssignedToUserId);
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName");

            //ProjectUserViewModels viewmodel = new ProjectUserViewModels();
            //var projectAssign = db.Projects.Where(p => p.AssignedToUserId == User.Identity.GetUserId()).OrderBy(p => p.AssignedToUser).ToList();
            ViewBag.AssignedToUser = new SelectList(db.Users, "Id", "FirstName", project.AssignedToUser);
            //viewmodel.Users = new SelectList(projectAssign, "Id", "DisplayName");
            //viewmodel.AssignProject = project;


            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit([Bind(Include = "Id,Name,AuthorId,Body,IsActive,DateCreated, DateUpdated, AssignedToUserId")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", project.AssignedToUserId);
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: EditProjectUsers
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult ProjectUser(int? id)
        {
            var project = db.Projects.Find(id);
            ProjectUserViewModels projectuserVM = new ProjectUserViewModels();
            projectuserVM.AssignProject = project;
            projectuserVM.AssignProjectId = Convert.ToInt32(project.Id);
            projectuserVM.SelectedUsers = project.Users.Select(u => u.Id).ToArray();
            projectuserVM.Users = new MultiSelectList(db.Users.ToList(), "Id", "FirstName", projectuserVM.SelectedUsers);
            // MultiSelectList parameters: collection of objects used for select list (user for us), inside user table we have diff properties that we submit by like Id or FirstName, display is FirstNames, highlighted selected users
            return View(projectuserVM); // how does this view return work? View(model) for example: passes in an object
        }

        // POST: EditProjectUsers
        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult ProjectUser(ProjectUserViewModels model)
        {
            // submit assign and it goes through here, we need to go through users assigned in database and put in the users selected
            ProjectAssignHelper projectassignhelper = new ProjectAssignHelper();
            //var project = db.Projects.Find(model.AssignProject.Id); No need for this since we don't need to find project in db by Id
            foreach (var userId in db.Users.Select(r => r.Id).ToList()) // remove all users from project
            {
                projectassignhelper.RemoveUserFromProject(userId, model.AssignProjectId);
            }
            foreach (var userId in model.SelectedUsers) // add back the ones you want
            {
                projectassignhelper.AddUserToProject(userId, model.AssignProjectId);
                ProjectUser projectuser = new ProjectUser();
                projectuser.ProjectId = model.AssignProjectId;
                projectuser.UserId = userId;
                db.ProjectUsers.Add(projectuser);
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = model.AssignProjectId });

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



    }
}
