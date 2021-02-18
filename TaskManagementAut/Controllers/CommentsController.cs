using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;
using TaskManagementAut.Models;

namespace TaskManagement.Controllers
{
    
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tasks
        [Authorize(Roles = "Organizator, Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Organizator, Admin")]
        public ActionResult Show(int id)
        {
            SetAccessRights(id);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View();
        }

        [Authorize(Roles = "User, Organizator, Admin")]
        public ActionResult Edit(int id)
        {
            var comm = db.Comments.Find(id);
            comm.CommentId = id;
            SetAccessRights(id);
            return View(comm);
        }

        //PUT: Edit
        [HttpPut]
        [Authorize(Roles = "User, Organizator, Admin")]
        public ActionResult Edit(int id, Comment requestComm)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    Comment comm = db.Comments.Find(id);
                    SetAccessRights(id);
                    if (TryUpdateModel(comm))
                    {
                        comm.Content = requestComm.Content;
                        comm.TaskId = requestComm.TaskId;
                        comm.Date = DateTime.Now;
                        db.SaveChanges();
                        TempData["message"] = "Comentariul a fost modificat cu succes!";

                    }
                    return Redirect("/Tasks/Show/" + comm.TaskId);
                }
                else
                {
                    var c = db.Comments.Find(id);
                    SetAccessRights(id);
                    //return Redirect("/Comments/Edit/" + c.CommentId);
                    return View(c);
                }
            }
            catch (Exception e)
            {
                var c = db.Comments.Find(id);
                //return Redirect("/Comments/Edit/" + c.CommentId);
                return View(c);
            }
        }

        //DELETE: Delete
        [Authorize(Roles = "User, Organizator, Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            db.Comments.Remove(comm);
            db.SaveChanges();
            TempData["message"] = "Comentariul a fost sters cu succes!";
            return Redirect("/Tasks/Show/" + comm.TaskId);
        }

        private void SetAccessRights(int CommentId)
        {
            var comm = db.Comments.Find(CommentId);
            var task = comm.Task;
            var proj = task.Project;
            var team = proj.Team;

            ViewBag.esteMembru = false;
            ViewBag.esteOrganizator = false;

            ViewBag.afisareButoane = false;
            if (User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }
            if (User.IsInRole("Organizator") && proj.UserId == User.Identity.GetUserId())
            {
                ViewBag.esteOrganizator = true;
                ViewBag.afisareButoane = true;
            }

            foreach (var mem in team.Members)
            {
                if (mem.UserId == User.Identity.GetUserId())
                {
                    ViewBag.afisareButoane = true;
                    ViewBag.esteMembru = true;
                }
                    
            }

            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
        }
    }
}
