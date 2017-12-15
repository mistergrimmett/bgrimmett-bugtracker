using bgrimmett_bugtracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace bgrimmett_bugtracker.Models
{
    public class AdminUserListModel
    {
        public ApplicationUser user { get; set; }
        public List<string> roles { get; set; }
    }
}