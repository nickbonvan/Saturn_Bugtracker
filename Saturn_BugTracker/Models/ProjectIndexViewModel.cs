using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Saturn_BugTracker.Models
{
    public class ProjectIndexViewModel
    {
        public List<Project> AllProjects { get; set; }
        public List<Project> UserProjects { get; set; }
    }
}