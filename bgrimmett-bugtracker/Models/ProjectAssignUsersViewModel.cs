using bgrimmett_bugtracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bgrimmett_bugtracker.Models
{
    public class ProjectAssignUsersViewModel
    {
        public Project Project { get; set; }

        public MultiSelectList Users { get; set; }
        public string[] SelectedUsers { get; set; }
    }
}
