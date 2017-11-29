using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Saturn_BugTracker.Models;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using System.Reflection;
using System.Collections;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Configuration;

namespace Saturn_BugTracker.Controllers
{
    public class TicketsController : UniversalController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Assignee).Include(t => t.Author).Include(t => t.Priority).Include(t => t.Project).Include(t => t.Status).Include(t => t.Type);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
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

        // GET: Tickets/Create
        public ActionResult Create(int ProjectId)
        {
            TicketCreateViewModel model = new TicketCreateViewModel();
            model.TypeId = new SelectList(db.Types, "Id", "Name");
            model.ProjectId = ProjectId;
            return View(model);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,TypeId,ProjectId")] Ticket ticket, int ProjectId, int TypeId)
        {
            if (ModelState.IsValid)
            {
                ticket.Created = DateTimeOffset.Now;
                //ViewBag.TypeId = new SelectList(db.Types, "Id", "Name");
                //ticket.ProjectId = ViewBag.ProjectId;
                ticket.AuthorId = User.Identity.GetUserId();
                ticket.PriorityId = 1;
                ticket.StatusId = 1;
                ticket.ProjectId = ProjectId;
                ticket.TypeId = TypeId;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Details", "Projects", new { id = ticket.ProjectId });
            }

            return RedirectToAction("Details", "Projects", new { id = ticket.ProjectId });
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket== null)
            {
                return HttpNotFound();
            }
            TicketEditViewModel model = new TicketEditViewModel();
            UserRolesHelper roles = new UserRolesHelper();
            model.Priorities = new SelectList(db.Priorities, "Id", "Name", ticket.PriorityId);
            model.Projects = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            model.Statuses = new SelectList(db.Statuses, "Id", "Name", ticket.StatusId);
            model.Types = new SelectList(db.Types, "Id", "Name", ticket.TypeId);
            model.Developers = new SelectList(roles.UsersInRole("Developer"), "Id", "FirstName", ticket.AssigneeId);
            model.Ticket = ticket;

            ViewBag.OldTitle = ticket.Title;
            ViewBag.OldDescription = ticket.Description;
            ViewBag.OldTypeId = ticket.TypeId;
            ViewBag.OldTypeName = ticket.Type.Name;
            ViewBag.OldPriorityId = ticket.PriorityId;
            ViewBag.OldPriorityName = ticket.Priority.Name;
            ViewBag.OldStatusId = ticket.StatusId;
            ViewBag.OldStatusName = ticket.Status.Name;
            ViewBag.OldAssigneeId = ticket.AssigneeId;
            if(ticket.Assignee != null)
            {
                ViewBag.OldAssigneeName = ticket.Assignee.DisplayName;
            }

            return View(model);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,ProjectId,TypeId,PriorityId,StatusId,AuthorId,AssigneeId")] Ticket ticket,
            string OldTitle, string OldDescription, int OldTypeId, string OldTypeName, int OldPriorityId, string OldPriorityName,
            int OldStatusId, string OldStatusName, string OldAssigneeId, string OldAssigneeName)
        {
            if (ModelState.IsValid)
            {
                ticket.Updated = DateTimeOffset.Now;
                Guid guid = new Guid();

                //This if block checks if there have been any changes to the ticket
                //If something has changed, it adds a new history item and updates the ticket

                //START IF BLOCK
                if (OldTitle != ticket.Title)
                {
                    UpdateTicket(OldTitle, ticket.Title, ticket, "Title", guid);
                }
                if (OldDescription != ticket.Description)
                {
                    UpdateTicket(OldDescription, ticket.Description, ticket, "Description", guid);
                }
                if (OldTypeId != ticket.TypeId)
                {
                    UpdateTicket(OldTypeName, db.Types.FirstOrDefault(type => type.Id == ticket.TypeId).Name, ticket, "Type", guid);
                }
                if (OldPriorityId != ticket.PriorityId)
                {
                    UpdateTicket(OldPriorityName, db.Priorities.FirstOrDefault(priority => priority.Id == ticket.PriorityId).Name, ticket, "Priority", guid);
                }
                if (OldStatusId != ticket.StatusId)
                {
                    UpdateTicket(OldStatusName, db.Statuses.FirstOrDefault(status => status.Id == ticket.StatusId).Name, ticket, "Status", guid);
                }
                if (OldAssigneeId != ticket.AssigneeId)
                {
                    UpdateTicket(OldAssigneeName, db.Users.FirstOrDefault(user => user.Id == ticket.AssigneeId).DisplayName, ticket, "Assignee", guid);
                }
                //END IF BLOCK

                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }
            return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
        }

        //GET
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Assign(int ticketId)
        {
            TicketAssignViewModel model = new TicketAssignViewModel();
            UserRolesHelper userRolesHelper = new UserRolesHelper();

            model.Developers = userRolesHelper.UsersInRole("Developer").ToList();
            model.Ticket = db.Tickets.Find(ticketId);
            model.Developers = model.Ticket.Project.Users.Where(user => model.Developers.Any(dev => dev.Id == user.Id)).ToList();
            model.DevelopersSelectList = new SelectList(model.Developers, "Id", "DisplayName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Assign(string userId, int ticketId)
        {
            int projectId = db.Projects.First(project => project.Tickets.Any(t => t.Id == ticketId)).Id;
            if(userId != null)
            {
                TicketAssignmentHelper ticketAssignmentHelper = new TicketAssignmentHelper(db);
                string assigneeName = db.Users.FirstOrDefault(user => user.Id == userId).DisplayName;
                Ticket ticket = db.Tickets.Find(ticketId);
                Guid updateId = new Guid();

                if (ticket.Assignee != null)
                {
                    UpdateTicket(ticket.Assignee.DisplayName, assigneeName, ticket, "Assignee", updateId);
                }
                else
                {
                    UpdateTicket("", assigneeName, ticket, "Assignee", updateId);
                }
                ticketAssignmentHelper.AssignUserToTicket(userId, ticketId);

                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                try
                {
                    string currentUserId = User.Identity.GetUserId();
                    ApplicationUser currentUser = db.Users.FirstOrDefault(user => user.Id == currentUserId);
                    ApplicationUser toUser = db.Users.FirstOrDefault(user => user.Id == userId);
                    var email = new MailMessage(ConfigurationManager.AppSettings["emailfrom"], toUser.Email)
                    {
                        Subject = String.Format("You have been assigned to ticket #{0}", ticket.Id),
                        Body = String.Format("Title: {0}\nType {1}\nStatus: {2}\nPriority: {3}\n",
                        ticket.Title, ticket.Type.Name, ticket.Status.Name, ticket.Priority.Name),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await Task.FromResult(0);
                }
                return RedirectToAction("Details", "Projects", new { id = projectId });
            }
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }

        private void UpdateTicket(string oldValue, string newValue, Ticket ticket, string property, Guid updateId)
        {
            History history = new History();
            ticket.Updated = DateTimeOffset.Now;
            history.AuthorId = User.Identity.GetUserId();
            history.Created = DateTimeOffset.Now;
            history.Group = updateId;
            history.NewValue = newValue;
            history.OldValue = oldValue;
            history.Property = property;
            history.TicketId = ticket.Id;
            db.History.Add(history);
            db.SaveChanges();

            //Notification notification = new Notification();
            //notification.Created = DateTimeOffset.Now;
            //notification.ReferenceId = updateId;
            ////notification.
        }

        // GET: Tickets/Delete/5
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
    }
}
