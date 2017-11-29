using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Saturn_BugTracker.Models
{
    public class UserRolesHelper
    {
        private UserManager<ApplicationUser> userManager =
            new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(
                    new ApplicationDbContext()));
        private RoleManager<IdentityRole> roleManager =
            new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(
                    new ApplicationDbContext()));

        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserInRole(string userId, string roleName) {
            return userManager.IsInRole(userId, roleName);
        }

        public ICollection<ApplicationUser> UsersInRole(string roleName)
        {
            var results = new List<ApplicationUser>();
            var List = userManager.Users.ToList();
            foreach (var user in List)
            {
                if (IsUserInRole(user.Id, roleName))
                    results.Add(user);
            }

            return results;
        }

        public string FindRoleNameById (string roleId)
        {
            var result = roleManager.FindById(roleId);
            return result.Name;
        }

        public string FindRoleIdByName(string roleName)
        {
            var result = roleManager.Roles.FirstOrDefault(r => r.Name == roleName); 
            return result.Id;
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            var result = userManager.AddToRole(userId, roleName);
            return result.Succeeded;
        }

        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = userManager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        public List<string> UserRoles(string userId)
        {
            return userManager.GetRoles(userId).ToList();
        }

        public IQueryable<IdentityRole> Roles()
        {
            return roleManager.Roles;
        }

        public List<string> RoleNames()
        {
            List<string> names = new List<string>();
            foreach(IdentityRole i in Roles())
            {
                names.Add(i.Name);
            }
            return names;
        }

        public List<string> RoleIds()
        {
            List<string> ids = new List<string>();
            foreach(IdentityRole i in Roles())
            {
                ids.Add(i.Id);
            }
            return ids;
        }
    }
}