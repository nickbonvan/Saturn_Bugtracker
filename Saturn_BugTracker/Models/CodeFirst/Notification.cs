using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Saturn_BugTracker.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int ReferenceId { get; set; }
        [Required]
        public string Message { get; set; }
        public string Type { get; set; }
        public DateTimeOffset Created { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}