using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Saturn_BugTracker.Models
{
    public class ProjectAssignmentHelper
    {
        private ApplicationDbContext db;

        public ProjectAssignmentHelper(ApplicationDbContext context)
        {
            db = context;
        }

        public bool UserInProject(int projectId, string userId)
        {
            var project = db.Projects.Find(projectId);
            return project.Users.Any(p => p.Id == userId);
        }

        public List<Project> UserProjects(string userId)
        {
            return db.Projects.Where(project => project.Users.Any(user => user.Id == userId)).ToList();
        }

        public ICollection<ApplicationUser> UsersInProject(int projectId)
        {
            Project project = db.Projects.Find(projectId);
            return project.Users.ToList();
        }

        public bool AddUserToProject(int projectId, string userId)
        {
            ApplicationUser currentUser = db.Users.Find(userId);
            Project project = db.Projects.Find(projectId);

            project.Users.Add(currentUser);

            try
            {
                var userAdded = db.SaveChanges();

                if (userAdded != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveUserFromProject(int projectId, string userId)
        {
            ApplicationUser currentUser = db.Users.Find(userId);
            Project project = db.Projects.Find(projectId);

            project.Users.Remove(currentUser);

            try
            {
                var userRemoved = db.SaveChanges();

                if (userRemoved != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}