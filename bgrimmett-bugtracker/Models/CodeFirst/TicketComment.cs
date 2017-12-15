using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bgrimmett_bugtracker.Models.CodeFirst
{
    public class TicketComment
    {

        public TicketComment()
        {
            
        }

        public int Id { get; set; }
        public string Body { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset? DateUpdated { get; set; }
        public int TicketId { get; set; }
        public string AuthorUserId { get; set; }

        public virtual ApplicationUser AuthorUser { get; set; }
        public virtual Ticket ticket { get; set; }

    }
}