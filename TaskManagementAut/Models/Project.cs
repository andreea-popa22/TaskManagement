using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagementAut.Models;

namespace TaskManagement.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 20 caractere!")]
        public string ProjectTitle { get; set; }

        [Required(ErrorMessage = "Descrierea articolului este obligatorie!")]
        [DataType(DataType.MultilineText)]
        public string ProjectDescription { get; set; }

        [Required(ErrorMessage = "Data inceperii proiectului este obligatorie!")]
        public DateTime ProjectDate { get; set; }

        [Required(ErrorMessage = "Echipa este obligatorie!")]
        public int TeamId { get; set; }

        public virtual Team Team { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }

        public IEnumerable<SelectListItem> Teamm { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; } //tipul userului
    }
}