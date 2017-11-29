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
using System.IO;

namespace Saturn_BugTracker.Controllers
{
    public class CommentsController : UniversalController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Author).Include(c => c.Ticket);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create(int ticketId)
        {
            ViewBag.AuthorId = User.Identity.GetUserId();
            ViewBag.TicketId = ticketId;
            ViewBag.ProjectId = db.Tickets.Find(ticketId).ProjectId;
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Body,TicketId")] Comment comment, string AuthorId, int ProjectId, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                foreach(var file in files)
                {
                    if(file != null && file.ContentLength != 0)
                    {
                        Attachment attachment = new Attachment();
                        //var fileName = Path.GetFileName(image.FileName);
                        var originalFileName = file.FileName;
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var uploadUrl = Server.MapPath("~/Uploads");

                        file.SaveAs(Path.Combine(uploadUrl, fileName));

                        attachment.Url = "Uploads/" + fileName;
                        attachment.TicketId = comment.TicketId;
                        attachment.AuthorId = AuthorId;
                        attachment.Created = DateTimeOffset.Now;
                        attachment.DisplayName = originalFileName;

                        comment.Attachments.Add(attachment);
                    }
                }

                comment.Created = DateTimeOffset.Now;
                comment.AuthorId = AuthorId;
                db.Comments.Add(comment);
                db.SaveChanges();

                Ticket ticket = db.Tickets.FirstOrDefault(t => t.Id == comment.TicketId);
                Guid guid = new Guid();
                ApplicationUser author = db.Users.FirstOrDefault(user => user.Id == comment.AuthorId);

                UpdateTicket("", author.DisplayName + " added comment " + comment.Id, ticket, "Comment", guid);
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }

            return RedirectToAction("Details", "Tickets", new { id = comment.TicketId });
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
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", comment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", comment.TicketId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Body,Created,TicketId,AuthorId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Updated = DateTimeOffset.Now;
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = comment.TicketId });
            }
            return RedirectToAction("Details", "Tickets", new { id = comment.TicketId });
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            
            Ticket ticket = db.Tickets.FirstOrDefault(t => t.Id == comment.TicketId);
            Guid guid = new Guid();
            ApplicationUser author = db.Users.FirstOrDefault(user => user.Id == comment.AuthorId);

            UpdateTicket(comment.Body, null, ticket, "Delete", guid);
            db.Entry(ticket).State = EntityState.Modified;

            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details", "Projects", ticket.Id);
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
