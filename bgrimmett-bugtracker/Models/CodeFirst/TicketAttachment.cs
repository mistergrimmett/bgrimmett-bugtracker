using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bgrimmett_bugtracker.Models.CodeFirst
{
    public class TicketAttachment
    {

        public TicketAttachment()
        {

        }

        public int Id { get; set; }
        public int TicketId { get; set; }
        public string MediaUrl { get; set; }
        public string Description { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string AuthorUserId { get; set; }

    }
}