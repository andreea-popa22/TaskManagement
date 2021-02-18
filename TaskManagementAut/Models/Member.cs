using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;

namespace TaskManagementAut.Models
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Userul este necesar")]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; } //tipul userului
        //public IEnumerable<SelectListItem> Userr { get; set; }
        
        public int TeamId { get; set; }

        public virtual Team Team { get; set; }
        //public IEnumerable<SelectListItem> Teamm { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }
}