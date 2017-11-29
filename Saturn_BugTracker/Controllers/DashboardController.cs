using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Saturn_BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace Saturn_BugTracker.Controllers
{
    public class DashboardController : UniversalController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index()
        {
            DashboardViewModel model = new DashboardViewModel();
            string userId = User.Identity.GetUserId();

            if(User.IsInRole("Admin") || User.IsInRole("Project Manager"))
            {
                model.RelevantProjects =
                    db.Projects.Where(
                        project => project.Users.Any(
                            user => user.Id == userId
                       )).ToList();

                model.IrrelevantProjects =
                    db.Projects.Where(
                        project => !project.Users.Any(
                            user => user.Id == userId
                            )).ToList();

                model.RelevantTickets =
                    db.Tickets.Where(ticket => ticket.AssigneeId == userId || ticket.AuthorId == userId ||
                    ticket.Project.Users.Any(user => user.Id == userId)).ToList();
            }

            model.RelevantProjects = 
                db.Projects.Where(
                    project => project.Users.Any(
                        user => user.Id == userId
                   )).ToList();

            model.IrrelevantProjects =
                db.Projects.Where(
                    project => !project.Users.Any(
                        user => user.Id == userId
                        )).ToList();

            model.RelevantTickets = db.Tickets.Where(
                ticket => ticket.AssigneeId == userId || ticket.AuthorId == userId
                ).ToList();

            model.IrrelevantTickets = db.Tickets.Where(
                ticket => ticket.AssigneeId != userId
                ).ToList();

            return View(model);
        }
    }
}