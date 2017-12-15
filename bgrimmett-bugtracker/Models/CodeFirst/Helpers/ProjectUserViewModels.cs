using bgrimmett_bugtracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bgrimmett_bugtracker.Models.Helpers
{
    public class ProjectUserViewModels
    {
        public Project AssignProject { get; set; }
        public int AssignProjectId { get; set; } // instead of passing Project AssignProject(the whole project) we pass just the Id
        public MultiSelectList Users { get; set; }
        public string[] SelectedUsers { get; set; }
    }
}