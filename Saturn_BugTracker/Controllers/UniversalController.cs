using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Saturn_BugTracker.Models;

namespace Saturn_BugTracker.Controllers
{
    public class UniversalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User != null)
            {
                var userId = User.Identity.GetUserId();

                //var context = new ApplicationDbContext();
                //var username = User.Identity.Name;
                if (!string.IsNullOrEmpty(userId))
                {
                    //var user = context.Users.SingleOrDefault(u => u.UserName == username);
                    ApplicationUser user = db.Users.FirstOrDefault(u => u.Id.Equals(userId));
                    UserRolesHelper userRolesHelper = new UserRolesHelper();
                    ViewBag.DisplayName = user.DisplayName;
                    ViewBag.Initials = user.FirstName[0].ToString() + user.LastName[0].ToString();
                    List<string> userRoles = userRolesHelper.UserRoles(user.Id);
                    if (userRoles.Contains("Admin"))
                    {
                        ViewBag.RoleName = "Admin";
                    }
                    else if (userRoles.Contains("Project Manager"))
                    {
                        ViewBag.RoleName = "Project Manager";
                    }
                    else if (userRoles.Contains("Developer"))
                    {
                        ViewBag.RoleName = "Developer";
                    }
                    else if (userRoles.Contains("Submitter"))
                    {
                        ViewBag.RoleName = "Submitter";
                    }
                    else
                    {
                        ViewBag.RoleName = "Unassigned";
                    }

                    //Doing this instead of adding a guest flag to the users in my database because
                    //I'm on the train and too tired to deal with Entity Framework right now

                    if (user.Email != "admin@bugtracker.com" &&
                        user.Email != "project.manager@bugtracker.com" &&
                        user.Email != "developer@bugtracker.com" &&
                        user.Email != "submitter@bugtracker.com")
                    {
                        ViewBag.DrawUserPortal = true;
                    }
                    else
                    {
                        ViewBag.DrawUserPortal = false;
                    }
                }
            }
            base.OnActionExecuted(filterContext);
        }
    }
}