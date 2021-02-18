using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TaskManagementAut.Models;

namespace TaskManagement.Models
{
    public class Task
    {

        [Key]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 20 caractere!")]
        public string TaskTitle { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie")]
        public string TaskDescription { get; set; }

        [Required(ErrorMessage = "Data inceperii taskului este obligatorie!")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Data de sfarsit a taskului este obligatorie!")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Statusul taskului este obligatoriu!")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Proiectul este obligatoriu!")]
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; } //tipul userului


    }
}