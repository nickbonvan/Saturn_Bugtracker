using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Saturn_BugTracker.Models
{
    public class ProjectDetailsViewModel
    {
        public Project Project { get; set; }
        public IDictionary<string, string> Roles { get; set; }

        public ProjectDetailsViewModel()
        {
            Roles = new Dictionary<string, string>();
        }
    }
}