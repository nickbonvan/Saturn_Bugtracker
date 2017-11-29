using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Saturn_BugTracker.Models
{
    public class ProjectAssignViewModel
    {
        public IDictionary<string, List<ApplicationUser>> UsersByRole { get; set; }
        public IDictionary<string, MultiSelectList> UsersByRoleMultiSelect { get; set; }
        public IDictionary<string, List<ApplicationUser>> AssignedUsersByRole { get; set; }
        public IDictionary<string, MultiSelectList> AssignedUsersByRoleMultiSelect { get; set; }
        public ICollection<string> RoleNames { get; set; }
        public Project Project { get; set; }

        public ProjectAssignViewModel()
        {
            UsersByRole = new Dictionary<string, List<ApplicationUser>>();
            AssignedUsersByRole = new Dictionary<string, List<ApplicationUser>>();
            UsersByRoleMultiSelect = new Dictionary<string, MultiSelectList>();
            AssignedUsersByRoleMultiSelect = new Dictionary<string, MultiSelectList>();
            RoleNames = new List<string>();
        }
    }
}