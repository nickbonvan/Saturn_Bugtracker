using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Saturn_BugTracker.Models
{
    public class TicketAssignViewModel
    {
        public List<ApplicationUser> Developers { get; set; }
        public SelectList DevelopersSelectList { get; set; }
        public Ticket Ticket { get; set; }
    }
}