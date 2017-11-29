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
using Microsoft.AspNet.Identity.EntityFramework;

namespace Saturn_BugTracker.Controllers
{
    public class ProjectsController : UniversalController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize]
        public ActionResult Index()
        {
            ApplicationUser currentUser = db.Users.Find(User.Identity.GetUserId());
            UserRolesHelper userRolesHelper = new UserRolesHelper();
            ProjectIndexViewModel model = new ProjectIndexViewModel();
            List<string> roles = new List<string>();

            foreach (string role in userRolesHelper.UserRoles(currentUser.Id))
            {
                roles.Add(role);
            }

            if (roles.Contains("Admin"))
            {
                model.AllProjects = db.Projects.ToList();
                model.UserProjects = db.Projects.Where(
                    project => 
                    project.Users.Any(
                        user => 
                        user.Id == currentUser.Id
                        )).ToList();
            }

            if(roles.Contains("Project Manager"))
            {
                model.AllProjects = db.Projects.ToList();
                model.UserProjects = db.Projects.Where(
                    project => 
                    project.Users.Any(
                        user => 
                        user.Id == currentUser.Id
                        )).ToList();
            }

            if (roles.Contains("Developer"))
            {
                model.UserProjects = db.Projects.Where(
                    project => 
                    project.Users.Any(
                        user => 
                        user.Id == currentUser.Id
                        )).ToList();
            }

            if (roles.Contains("Submitter"))
            {
                model.UserProjects = db.Projects.Where(
                    project =>
                    project.Users.Any(
                        user =>
                        user.Id == currentUser.Id
                        )).ToList();
            }

            model.UserProjects = model.UserProjects.Distinct().ToList();
            return View(model);
        }

        // GET: Projects/Details/5
        [Authorize]
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

            ProjectDetailsViewModel model = new ProjectDetailsViewModel();
            UserRolesHelper userRolesHelper = new UserRolesHelper();
            model.Project = project;

            foreach(IdentityRole role in userRolesHelper.Roles())
            {
                model.Roles.Add(role.Id, role.Name);
            }
            return View(model);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                string creatorId = User.Identity.GetUserId();
                project.Created = DateTimeOffset.Now;
                project.Users.Add(db.Users.FirstOrDefault(user => user.Id == creatorId));
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = project.Id });
            }

            return RedirectToAction("Details", new { id = project.Id });
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
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Updated = DateTimeOffset.Now;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = project.Id });
            }
            return RedirectToAction("Index", "Dashboard");
        }

        //GET
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Assign(int projectId)
        {
            ProjectAssignViewModel model = new ProjectAssignViewModel();
            UserRolesHelper userRolesHelper = new UserRolesHelper();
            List<string> roleNames = userRolesHelper.RoleNames();
            model.Project = db.Projects.Find(projectId);
            model.RoleNames = roleNames;

            //Users Not In Project
            foreach(string roleName in roleNames)
            {
                List<ApplicationUser> unassignedUsers = 
                    db.Users.Where(user => 
                    !user.Projects.Any(project => 
                        project.Id == projectId)).ToList().Where(user => 
                    userRolesHelper.UserRoles(user.Id).Any(
                        role => role == roleName))
                    .ToList();
                List<ApplicationUser> usersInRole = new List<ApplicationUser>();
                foreach (ApplicationUser user in unassignedUsers)
                {
                    usersInRole.Add(user);
                }
                model.UsersByRole.Add(roleName, usersInRole);
                model.UsersByRoleMultiSelect.Add(roleName, new MultiSelectList(usersInRole, "Id", "DisplayName"));
            }

            foreach (string roleName in roleNames)
            {
                List<ApplicationUser> usersInRole = new List<ApplicationUser>();
                foreach (ApplicationUser user in model.Project.Users)
                {
                    List<string> userRoles = userRolesHelper.UserRoles(user.Id);
                    if(userRoles.Any(role => role == roleName))
                    {
                        usersInRole.Add(user);
                    }
                }
                model.AssignedUsersByRole.Add(roleName, usersInRole);
                model.AssignedUsersByRoleMultiSelect.Add(roleName, new MultiSelectList(usersInRole, "Id", "DisplayName"));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(IEnumerable<string> users, int id)
        {
            if(users != null)
            {
                ProjectAssignmentHelper projectAssignmentHelper = new ProjectAssignmentHelper(db);
                Project project = db.Projects.FirstOrDefault(p => p.Id == id);
                foreach (string userId in users)
                {
                    projectAssignmentHelper.AddUserToProject(project.Id, userId);
                }
                project.Updated = DateTimeOffset.Now;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
            }

            ProjectAssignViewModel model = new ProjectAssignViewModel();
            UserRolesHelper userRolesHelper = new UserRolesHelper();
            List<string> roleNames = userRolesHelper.RoleNames();
            model.Project = db.Projects.Find(id);
            model.RoleNames = roleNames;

            foreach (string roleName in roleNames)
            {
                List<ApplicationUser> usersInRole = new List<ApplicationUser>();
                foreach (ApplicationUser user in userRolesHelper.UsersInRole(roleName))
                {
                    usersInRole.Add(user);
                }
                model.UsersByRole.Add(roleName, usersInRole);
                model.UsersByRoleMultiSelect.Add(roleName, new MultiSelectList(usersInRole, "Id", "DisplayName"));
            }

            foreach (string roleName in roleNames)
            {
                List<ApplicationUser> usersInRole = new List<ApplicationUser>();
                foreach (ApplicationUser user in model.Project.Users)
                {
                    List<string> userRoles = userRolesHelper.UserRoles(user.Id);
                    if (userRoles.Any(role => role == roleName))
                    {
                        usersInRole.Add(user);
                    }
                }
                model.AssignedUsersByRole.Add(roleName, usersInRole);
            }
            ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unassign(IEnumerable<string> users, int id)
        {
            if(users != null)
            {
            ProjectAssignmentHelper projectAssignmentHelper = new ProjectAssignmentHelper(db);
            Project project = db.Projects.FirstOrDefault(p => p.Id == id);
            foreach (string userId in users)
            {
                projectAssignmentHelper.RemoveUserFromProject(project.Id, userId);
            }
            project.Updated = DateTimeOffset.Now;
            db.Entry(project).State = EntityState.Modified;
            db.SaveChanges();
            }

            ProjectAssignViewModel model = new ProjectAssignViewModel();
            UserRolesHelper userRolesHelper = new UserRolesHelper();
            List<string> roleNames = userRolesHelper.RoleNames();
            model.Project = db.Projects.Find(id);
            model.RoleNames = roleNames;

            foreach (string roleName in roleNames)
            {
                List<ApplicationUser> usersInRole = new List<ApplicationUser>();
                foreach (ApplicationUser user in userRolesHelper.UsersInRole(roleName))
                {
                    usersInRole.Add(user);
                }
                model.UsersByRole.Add(roleName, usersInRole);
                model.UsersByRoleMultiSelect.Add(roleName, new MultiSelectList(usersInRole, "Id", "DisplayName"));
            }

            foreach (string roleName in roleNames)
            {
                List<ApplicationUser> usersInRole = new List<ApplicationUser>();
                foreach (ApplicationUser user in model.Project.Users)
                {
                    List<string> userRoles = userRolesHelper.UserRoles(user.Id);
                    if (userRoles.Any(role => role == roleName))
                    {
                        usersInRole.Add(user);
                    }
                }
                model.AssignedUsersByRole.Add(roleName, usersInRole);
            }
            ModelState.Clear();
            return View(model);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, Project Manager")]
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
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index", "Dashboard");
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