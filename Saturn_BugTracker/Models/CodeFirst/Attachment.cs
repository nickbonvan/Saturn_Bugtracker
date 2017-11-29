using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Saturn_BugTracker.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        [Required]
        public string Url { get; set; }
        public string DisplayName { get; set; }
        public DateTimeOffset Created { get; set; }

        public int TicketId { get; set; }
        public string AuthorId { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Author { get; set; }
    }
}