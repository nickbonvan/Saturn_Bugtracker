using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Saturn_BugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int TypeId { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }
        public string AuthorId { get; set; }
        public string AssigneeId { get; set; }

        public virtual ICollection<History> History { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual Project Project { get; set; }
        public virtual Type Type { get; set; }
        public virtual Priority Priority { get; set; }
        public virtual Status Status { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual ApplicationUser Assignee { get; set; }

        public Ticket()
        {
            History = new HashSet<History>();
            Attachments = new HashSet<Attachment>();
            Comments = new HashSet<Comment>();
        }
    }
}