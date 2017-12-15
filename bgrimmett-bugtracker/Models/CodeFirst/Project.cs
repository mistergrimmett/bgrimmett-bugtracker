using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bgrimmett_bugtracker.Models.CodeFirst
{
    public class Project
    {
        //Default constructor
        public Project()
        {
            this.Tickets = new HashSet<Ticket>();
            this.Users = new HashSet<ApplicationUser>();
            this.IsActive = true;


        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string AuthorId { get; set; }
        [Required]
        public string Body { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public DateTimeOffset? DateUpdated { get; set; }
        public string AssignedToUserId { get; set; }

        public virtual ApplicationUser AssignedToUser { get; set; }
        public virtual ApplicationUser Author { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}