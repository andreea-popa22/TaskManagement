using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TaskManagementAut.Models;

namespace TaskManagement.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(100, ErrorMessage = "Numele nu poate avea mai mult de 20 caractere!")]
        public string TeamName { get; set; }

        public virtual ICollection<Member> Members { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; } //tipul userului
    }
}