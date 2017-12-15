using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bgrimmett_bugtracker.Models.CodeFirst
{
    public class Ticket
    {

        public Ticket()
        {
            this.TicketComments = new HashSet<TicketComment>();
            this.TicketAttachments = new HashSet<TicketAttachment>();
            this.TicketHistories = new HashSet<TicketHistory>();
            this.Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset? DateUpdated { get; set; }
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }

        public string AuthorUserId { get; set; }
        public string AssignedToUserId { get; set; }

        public virtual TicketType TicketType { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }

        //Virtual = Navigation property
        public virtual ApplicationUser AuthorUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }
        public virtual Project Project { get; set; }

        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

    }
}