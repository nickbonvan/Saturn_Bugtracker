using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Saturn_BugTracker.Models
{
    public class TicketEditViewModel
    {
        public SelectList Priorities { get; set; }
        public SelectList Projects { get; set; }
        public SelectList Statuses { get; set; }
        public SelectList Types { get; set; }
        public SelectList Developers { get; set; }
        public Ticket Ticket { get; set; }
    }
}