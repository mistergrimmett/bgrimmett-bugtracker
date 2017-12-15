using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bgrimmett_bugtracker.Models.CodeFirst
{
    public class Notification
    {

        public int Id { get; set; }
        public int TicketId { get; set; }
        [Required]
        public string Message { get; set; }
        public string Type { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string UserId { get; set; }
        public bool? justCreated { get; set; }

        public virtual ApplicationUser Users { get; set; }
    }
}