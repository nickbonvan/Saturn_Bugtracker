using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Saturn_BugTracker.Models
{
    public class TicketAssignmentHelper
    {
        private ApplicationDbContext db;

        public TicketAssignmentHelper(ApplicationDbContext context)
        {
            db = context;
        }

        public bool AssignUserToTicket(string userId, int ticketId)
        {
            ApplicationUser currentUser = db.Users.Find(userId);
            Ticket ticket = db.Tickets.Find(ticketId);

            ticket.Assignee = currentUser;
            ticket.Updated = DateTimeOffset.Now;
            try
            {
                var userAdded = db.SaveChanges();

                if (userAdded != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool UnassignUserFromTicket(string userId, int ticketId)
        {
            Ticket ticket = db.Tickets.Find(ticketId);
            ticket.Assignee = null;
            ticket.Updated = DateTimeOffset.Now;

            try
            {
                var userRemoved = db.SaveChanges();

                if (userRemoved != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Ticket> AssignedToUser(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            List<Ticket> tickets = db.Tickets.Where(ticket => ticket.Assignee.Id == userId).ToList();

            return tickets;
        }

        public IEnumerable<Ticket> WrittenByUser(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            List<Ticket> tickets = db.Tickets.Where(ticket => ticket.Author.Id == userId).ToList();

            return tickets;
        }
    }
}