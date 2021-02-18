using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TaskManagementAut.Models;

namespace TaskManagement.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        [StringLength(200, ErrorMessage = "Comentariul nu poate avea mai mult de 200 caractere!")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Taskul este obligatoriu!")]
        public int TaskId { get; set; }
        public virtual Task Task { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; } //tipul userului

        

    }
}