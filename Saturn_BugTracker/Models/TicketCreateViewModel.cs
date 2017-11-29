using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Saturn_BugTracker.Models
{
    public class TicketCreateViewModel
    {
        public Ticket Ticket { get; set; }
        public int ProjectId { get; set; }
        public SelectList TypeId { get; set; }
    }
}