using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Saturn_BugTracker.Models
{
    public class TicketShallowClone
    {
        public string OldTitle        { get; set; }
        public string OldDescription  { get; set; }
        public int OldTypeId          { get; set; }
        public string OldTypeName     { get; set; }
        public int OldPriorityId      { get; set; }
        public string OldPriorityName { get; set; }
        public int OldStatusId        { get; set; }
        public string OldStatusName   { get; set; }
        public string OldAssigneeId   { get; set; }
        public string OldAssigneeName { get; set; }

        public TicketShallowClone(Ticket ticket)
        {
            OldTitle = ticket.Title;
            OldDescription = ticket.Description;
            OldTypeId = ticket.TypeId;
            OldTypeName = ticket.Type.Name;
            OldPriorityId = ticket.PriorityId;
            OldPriorityName = ticket.Priority.Name;
            OldStatusId = ticket.StatusId;
            OldStatusName = ticket.Status.Name;
            OldAssigneeId = ticket.AssigneeId;
            if(ticket.Assignee != null)
            {
                OldAssigneeName = ticket.Assignee.DisplayName;
            }
            else
            {
                OldAssigneeName = null;
            }
        }

        public TicketShallowClone(TicketShallowClone ticket)
        {
            OldTitle = ticket.OldTitle;
            OldDescription = ticket.OldDescription;
            OldTypeId = ticket.OldTypeId;
            OldTypeName = ticket.OldTypeName;
            OldPriorityId = ticket.OldPriorityId;
            OldPriorityName = ticket.OldPriorityName;
            OldStatusId = ticket.OldStatusId;
            OldStatusName = ticket.OldStatusName;
            OldAssigneeId = ticket.OldAssigneeId;
            OldAssigneeName = ticket.OldAssigneeName;
            OldAssigneeName = null;
        }
    }
}