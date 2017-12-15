using System;
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
using PagedList.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using bgrimmett_bugtracker.Models.CodeFirst.Helpers;
using System.Reflection;
using System.Data.Entity.Infrastructure;

namespace bgrimmett_bugtracker.Controllers
{
    public class TicketsController : Universal
    {
        //Properties for creating a ticket
        const string TicketHistoryPropertyCreated = "Ticket Created";

        //Properties for updating a ticket
        const string TicketTitleUpdate = "Ticket Title Updated";
        const string TicketBodyUpdate = "Ticket Body Updated";
        const string TicketStatusUpdate = "Ticket Status Updated";
        const string TicketPriorityUpdate = "Ticket Priority Updated";
        const string TicketTypeUpdate = "Ticket Type Updated";

        //private ApplicationDbContext db = new ApplicationDbContext();
        //private Ticket currentTicket = new Ticket();

        public ActionResult SearchResults(string searchitem)
        {

            if (searchitem == null)
            {
                return HttpNotFound();
            }

            return View(db.Tickets.Where(i => i.Title.Contains(searchitem) || i.AuthorUserId.Contains(searchitem) || i.Body.Contains(searchitem) || i.AssignedToUserId.Contains(searchitem)).ToList());


        }

        // GET: Tickets
        public ActionResult Index(int? id, int? page)
        {


            int pageSize = 5;// display three blog posts at a time on this page
            int pageNumber = (page ?? 1);
            var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.AuthorUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).OrderByDescending(t => t.DateCreated);
            return View(tickets.ToPagedList(pageNumber, pageSize));
        }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        //GET: Tickets/MyTickets
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        // GET: Assigned Tickets to user
        public ActionResult MyTickets(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var user = db.Users.Find(User.Identity.GetUserId());

            return View(db.Tickets.Where(a => user.Id.Contains(a.AssignedToUserId)).OrderByDescending(p => p.DateCreated).ToPagedList(pageNumber, pageSize));
        }


        // GET: Tickets/Create
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Create()
        {
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.AuthorUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult Create([Bind(Include = "Id,Title,Body,DateCreated,DateUpdated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,AuthorUserId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser currentUser = db.Users.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name));

                var user = db.Users.Find(User.Identity.GetUserId());
                ticket.TicketStatusId = 1;
                ticket.AuthorUserId = user.Id;
                ticket.DateCreated = System.DateTimeOffset.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();

                //Add History Here
                NewHistoryCreate(ticket, currentUser);

                //Add Notification Logic here
                NewNotificationCreate(ticket);


                return RedirectToAction("Index");
            }

            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.AuthorUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AuthorUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }



        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            //this.currentTicket = ticket;
            if (ticket == null)
            {
                return HttpNotFound();
            }

            var role = db.Roles.SingleOrDefault(u => u.Name == "Developer");
            var usersInRole = db.Users.Where(u => u.Roles.Any(r => (r.RoleId == role.Id)));
            ViewBag.AssignedToUserId = new SelectList(usersInRole, "Id", "FirstName");

            //ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.AuthorUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AuthorUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,DateCreated,DateUpdated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,AuthorUserId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.DateUpdated = System.DateTimeOffset.Now;

                //Make new TicketHistory
                //get a non-proxy oldTicket
                var dbNoTrack = new ApplicationDbContext();
                ((IObjectContextAdapter)dbNoTrack).ObjectContext.ContextOptions.ProxyCreationEnabled = false;
                var oldTicket = dbNoTrack.Tickets.Find(ticket.Id);
                //Check and record changes
                CheckChanged(oldTicket, ticket);
                // get rid of non-proxy old ticket
                dbNoTrack.Dispose();
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();



                return RedirectToAction("Index");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.AuthorUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AuthorUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        public void CheckChanged(object first, object second)
        {
            Type firstType = first.GetType();
            foreach (PropertyInfo propertyInfo in firstType.GetProperties())
            {
                if (propertyInfo.CanRead)
                {
                    object firstValue = propertyInfo.GetValue(first, null);
                    object secondValue = propertyInfo.GetValue(second, null);
                    if (firstValue != null || secondValue != null)
                    {
                        if (firstValue == null || secondValue == null || !firstValue.Equals(secondValue))
                        {
                            string firstV = null;
                            string secondV = null;

                            if (firstValue == null)
                                firstV = null;
                            else
                                firstV = firstValue.ToString();

                            if (secondValue == null)
                                secondV = null;
                            else
                                secondV = secondValue.ToString();
                            if (firstV == null || secondV == null || !firstV.Equals(secondV))
                            {
                                if (propertyInfo.Name != "TicketHistories" && propertyInfo.Name != "DateUpdated" && propertyInfo.Name != "DateCreated")
                                {
                                    updateHistory(propertyInfo.Name,
                                        first as Ticket,
                                        second as Ticket,
                                        firstV,
                                        secondV
                                        );
                                }
                            }
                        }
                    }
                }
            }
        }

        private void updateHistory(string property, Ticket old, Ticket current, string oldProp, string newProp)
        {
            db.TicketHistories.Add(new TicketHistory()
            {
                TicketId = current.Id,
                UserId = User.Identity.GetUserId(),
                Property = property,
                OldValue = oldProp,
                NewValue = newProp,
                ChangeDate = DateTimeOffset.Now,
            });
            db.SaveChanges();
        }




        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin, Submitter")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Submitter")]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        //[RequireHttps]
        [ValidateAntiForgeryToken]
        public ActionResult CommentCreate([Bind(Include = "Id,Body,MediaURL,DateCreated,DateUpdated,TicketId,AuthorUserId")] TicketComment ticketcomment, int? id, HttpPostedFileBase image)
        { // only pass in the bind the attributes that have forms
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {

                if (!String.IsNullOrWhiteSpace(ticketcomment.Body))
                {
                    var commentId = db.Tickets.Find(ticketcomment.TicketId);
                    ticketcomment.DateCreated = System.DateTime.Now;
                    ticketcomment.AuthorUserId = User.Identity.GetUserId();
                    db.TicketComments.Add(ticketcomment);
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = commentId.Id });
                }
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        //[RequireHttps]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult CreateAttachment([Bind(Include = "Id,TicketId,MediaUrl,Description,DateCreated,AuthorUserId")] TicketAttachment ticketattachment, HttpPostedFileBase image)
        { // only pass in the bind the attributes that have forms
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength > 0)
                {
                    var ext = Path.GetExtension(image.FileName).ToLower();
                    if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp" && ext != ".pdf" && ext != ".doc" && ext != ".docx" && ext != ".txt" && ext != ".html")
                        ModelState.AddModelError("image", "Invaild Format.");
                }
                if (!String.IsNullOrWhiteSpace(ticketattachment.Description))
                {
                    var ticket = db.Tickets.Find(ticketattachment.TicketId);

                    if (image != null)
                    {

                        var filepath = "/startbootstrap-sb-admin-gh-pages/Uploads/";
                        var absPath = Server.MapPath("~" + filepath);

                        if (ticketattachment.MediaUrl != string.Empty)
                        {
                            ticketattachment.MediaUrl = filepath + image.FileName;
                            image.SaveAs(Path.Combine(absPath, image.FileName));
                        }
                    }
                    ticketattachment.DateCreated = System.DateTimeOffset.Now;
                    ticketattachment.AuthorUserId = User.Identity.GetUserId();
                    db.TicketAttachments.Add(ticketattachment);
                    db.SaveChanges();

                    return RedirectToAction("Details", new { id = ticket.Id });
                }
            }

            return RedirectToAction("Index");
        }

        private void NewHistoryCreate(Ticket ticket, ApplicationUser currentUser)
        {
            TicketHistory history = new TicketHistory();

            history.TicketId = ticket.Id;

            if (currentUser != null)
            {
                history.UserId = currentUser.Id;
            }

            history.Property = TicketHistoryPropertyCreated;
            history.OldValue = "";
            history.NewValue = TicketHistoryPropertyCreated;
            history.ChangeDate = DateTimeOffset.Now;
            history.UserId = User.Identity.GetUserId();
            db.TicketHistories.Add(history);
            db.SaveChanges();
        }

        private void NewNotificationCreate(Ticket ticket)
        {
            Notification notification = new Notification();

            notification.TicketId = ticket.Id;

            
            notification.DateCreated = DateTimeOffset.Now;
            notification.UserId = User.Identity.GetUserId();
            db.Notifications.Add(notification);
            db.SaveChanges();
        }

    }
}
