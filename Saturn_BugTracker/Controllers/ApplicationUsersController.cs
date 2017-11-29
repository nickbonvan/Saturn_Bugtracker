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
    public class ApplicationUsersController : UniversalController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ApplicationUsers
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            List<ApplicationUser> users = db.Users.Where(user =>
            user.Email != "admin@bugtracker.com" &&
            user.Email != "project.manager@bugtracker.com" &&
            user.Email != "developer@bugtracker.com" &&
            user.Email != "submitter@bugtracker.com" &&
            user.Email != "nicholas.bonvan@gmail.com").ToList();
            
            return View(users);
        }

        // GET: ApplicationUsers/Details/5
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            UserRolesHelper userRolesHelper = new UserRolesHelper();
            List<IdentityRole> UserRoles = new List<IdentityRole>();

            ViewBag.UserRoles = userRolesHelper.UserRoles(id);
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,DisplayName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(applicationUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            List<string> otherRoles = new List<string>();
            List<string> currentRoles = new List<string>();
            UserRolesHelper userRolesHelper = new UserRolesHelper();
            foreach (IdentityUserRole r in applicationUser.Roles)
            {
                currentRoles.Add(userRolesHelper.FindRoleNameById(r.RoleId));
            }
            foreach(IdentityRole role in db.Roles)
            {
                if (!currentRoles.Contains(userRolesHelper.FindRoleNameById(role.Id))){
                    otherRoles.Add(role.Name);
                }
            }
            ViewBag.Roles = new MultiSelectList(otherRoles);
            ViewBag.CurrentRoles = new MultiSelectList(currentRoles);
            ViewBag.RolesCount = db.Roles.Count();

            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,DisplayName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser, List<string> roles, List<string> CurrentRoles)
        {
            //ViewBag.Roles = new MultiSelectList(db.Roles, "Id", "Name");
            
            if (ModelState.IsValid)
            {
                UserRolesHelper userRolesHelper = new UserRolesHelper();
                if(CurrentRoles != null)
                {
                    foreach (string role in CurrentRoles)
                    {
                        userRolesHelper.RemoveUserFromRole(applicationUser.Id, role);
                    }
                }
                
                if(roles != null)
                {
                    foreach (string role in roles)
                    {
                        string userId = applicationUser.Id;
                        userRolesHelper.AddUserToRole(userId, role);
                    }
                }
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = applicationUser.Id });
            }
            return RedirectToAction("Index");
        }

        // GET: ApplicationUsers/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
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
